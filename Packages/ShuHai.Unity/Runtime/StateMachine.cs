using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ShuHai.Unity
{
    /// <summary>
    ///     Provides states and state transition management of specified state type with coroutine support.
    /// </summary>
    /// <typeparam name="T">The enum type representing the state value.</typeparam>
    public class StateMachine<T>
        where T : struct, Enum
    {
        public StateMachine() : this(default) { }

        /// <summary>
        ///     Initialize a new state machine with specified initial state.
        /// </summary>
        /// <param name="initialState">The value of initial state.</param>
        public StateMachine(T initialState)
        {
            _state = initialState;

            InitializeStateEvents();
            InitializeTransitions();
        }

        #region State

        public T State
        {
            get => _state;
            set
            {
                var e = ChangeState(value, null);
                while (e.MoveNext()) { }
            }
        }

        /// <summary>
        ///     Change current state of current state machine with Unity's coroutine support.
        /// </summary>
        /// <param name="newState">The desired state after change.</param>
        /// <param name="args">Arguments used when triggering state change events.</param>
        /// <returns>
        ///     <see langword="true" /> if state of current state machine changed; otherwise <see langword="false" /> if
        ///     current state of current state machine is the same as desired <paramref name="newState" />.
        /// </returns>
        public IEnumerator ChangeState(T newState, params object[] args)
        {
            T src = _state, dst = newState;
            if (EqualityComparer<T>.Default.Equals(src, dst))
                yield break;

            PrepareStateChange(src, dst, args);

            foreach (var routine in _stateChangingRoutines)
            {
                while (routine.MoveNext())
                    yield return routine.Current;
            }

            _state = dst;

            foreach (var routine in _stateChangedRoutines)
            {
                while (routine.MoveNext())
                    yield return routine.Current;
            }

            FinishStateChange();
        }

        private T _state;

        private readonly List<IEnumerator> _stateChangingRoutines = new List<IEnumerator>(10);
        private readonly List<IEnumerator> _stateChangedRoutines = new List<IEnumerator>(10);

        private void PrepareStateChange(T src, T dst, object[] args)
        {
            StateEvent srcEvt = GetStateEvent(src), dstEvt = GetStateEvent(dst);
            _transitings[0] = GetTransition(src, dst);
            _transitings[1] = GetTransition(src, null);
            _transitings[2] = GetTransition(null, dst);
            _transitings[3] = GetTransition(null, null);

            _stateChangingRoutines.AddRange(srcEvt.CreateExitingRoutines(src, dst));
            _stateChangingRoutines.AddRange(dstEvt.CreateEnteringRoutines(src, dst, args));
            foreach (var t in _transitings)
                _stateChangingRoutines.AddRange(t.CreateStateChangingRoutines(src, dst, args));

            foreach (var t in _transitings.Reverse())
                _stateChangedRoutines.AddRange(t.CreateStateChangedRoutines(src, dst, args));
            _stateChangedRoutines.AddRange(srcEvt.CreateExitedRoutines(src, dst));
            _stateChangedRoutines.AddRange(dstEvt.CreateEnteredRoutines(src, dst, args));
        }

        private void FinishStateChange()
        {
            _stateChangingRoutines.Clear();
            _stateChangedRoutines.Clear();

            for (int i = 0; i < _transitings.Length; ++i)
                _transitings[i] = null;
        }

        private readonly Transition[] _transitings = new Transition[4];

        #endregion State

        #region State Events

        public sealed class StateEvent
        {
            public List<Func<StateEvent, T, T, object[], IEnumerator>>
                EnteringRoutines { get; } = new List<Func<StateEvent, T, T, object[], IEnumerator>>();

            public List<Func<StateEvent, T, T, object[], IEnumerator>>
                EnteredRoutines { get; } = new List<Func<StateEvent, T, T, object[], IEnumerator>>();

            public List<Func<StateEvent, T, T, IEnumerator>>
                ExitingRoutines { get; } = new List<Func<StateEvent, T, T, IEnumerator>>();

            public List<Func<StateEvent, T, T, IEnumerator>>
                ExitedRoutines { get; } = new List<Func<StateEvent, T, T, IEnumerator>>();

            public T State { get; }

            internal StateEvent(T state) { State = state; }

            internal IEnumerable<IEnumerator> CreateEnteringRoutines(T src, T dst, object[] args)
            {
                return EnteringRoutines.Select(r => r?.Invoke(this, src, dst, args));
            }

            internal IEnumerable<IEnumerator> CreateEnteredRoutines(T src, T dst, object[] args)
            {
                return EnteredRoutines.Select(r => r?.Invoke(this, src, dst, args));
            }

            internal IEnumerable<IEnumerator> CreateExitingRoutines(T src, T dst)
            {
                return ExitingRoutines.Select(r => r?.Invoke(this, src, dst));
            }

            internal IEnumerable<IEnumerator> CreateExitedRoutines(T src, T dst)
            {
                return ExitedRoutines.Select(r => r?.Invoke(this, src, dst));
            }
        }

        public StateEvent GetStateEvent(T state) { return _stateEvents[EnumTraits<T>.IndexOf(state)]; }

        private readonly StateEvent[] _stateEvents = new StateEvent[EnumTraits<T>.ElementCount];

        private void InitializeStateEvents()
        {
            for (int i = 0; i < EnumTraits<T>.ElementCount; ++i)
                _stateEvents[i] = new StateEvent(EnumTraits<T>.Values[i]);
        }

        #endregion State Events

        #region Transitions

        /// <summary>
        ///     Represents a state transition.
        /// </summary>
        public sealed class Transition
        {
            public List<Func<Transition, T, T, object[], IEnumerator>>
                StateChangingRoutines { get; } = new List<Func<Transition, T, T, object[], IEnumerator>>();

            public List<Func<Transition, T, T, object[], IEnumerator>>
                StateChangedRoutines { get; } = new List<Func<Transition, T, T, object[], IEnumerator>>();

            /// <summary>
            ///     The source state from which the current transition changes from.
            ///     A value of <see langword="null" /> indicates any state.
            /// </summary>
            public T? SourceState { get; }

            /// <summary>
            ///     The destination state to which the current transition changes to.
            ///     A value of <see langword="null" /> indicates any state.
            /// </summary>
            public T? DestinationState { get; }

            /// <summary>
            ///     The owner state machine of current transition.
            /// </summary>
            public StateMachine<T> Owner { get; }

            internal Transition(StateMachine<T> owner, T? sourceState, T? destinationState)
            {
                Owner = owner;
                SourceState = sourceState;
                DestinationState = destinationState;
            }

            internal IEnumerable<IEnumerator> CreateStateChangingRoutines(T src, T dst, object[] args)
            {
                return StateChangingRoutines.Select(r => r?.Invoke(this, src, dst, args));
            }

            internal IEnumerable<IEnumerator> CreateStateChangedRoutines(T src, T dst, object[] args)
            {
                return StateChangedRoutines.Select(r => r?.Invoke(this, src, dst, args));
            }
        }

        /// <summary>
        ///     Get the transition object representing the state transition from the specified source state to destination state.
        /// </summary>
        /// <param name="sourceState">
        ///     The state before the transition occuring. A value of <see langword="null" /> refers to any state.
        /// </param>
        /// <param name="destinationState">
        ///     The state after the transition occured. A value of <see langword="null" /> refers to any state.
        /// </param>
        public Transition GetTransition(T? sourceState, T? destinationState)
        {
            int srcIndex = sourceState.HasValue ? EnumTraits<T>.IndexOf(sourceState.Value) : Index.Invalid,
                dstIndex = destinationState.HasValue ? EnumTraits<T>.IndexOf(destinationState.Value) : Index.Invalid;

            if (sourceState.HasValue && destinationState.HasValue)
                return _srcToDstTransitions[srcIndex, dstIndex];
            if (sourceState.HasValue) // && !destinationState.HasValue
                return _srcToAnyTransitions[srcIndex];
            if (destinationState.HasValue) // && !sourceState.HasValue
                return _anyToDstTransitions[dstIndex];
            return _anyToAnyTransition; // !sourceState.HasValue && !destinationState.HasValue
        }

        private readonly Transition[,] _srcToDstTransitions =
            new Transition[EnumTraits<T>.ElementCount, EnumTraits<T>.ElementCount];

        private readonly Transition[] _srcToAnyTransitions = new Transition[EnumTraits<T>.ElementCount];
        private readonly Transition[] _anyToDstTransitions = new Transition[EnumTraits<T>.ElementCount];
        private Transition _anyToAnyTransition;

        private void InitializeTransitions()
        {
            int stateCount = EnumTraits<T>.ElementCount;

            for (var srcIndex = 0; srcIndex < stateCount; ++srcIndex)
            {
                var src = EnumTraits<T>.Values[srcIndex];
                for (var dstIndex = 0; dstIndex < stateCount; ++dstIndex)
                {
                    var dst = EnumTraits<T>.Values[dstIndex];
                    _srcToDstTransitions[srcIndex, dstIndex] = new Transition(this, src, dst);
                }
            }

            for (var srcIndex = 0; srcIndex < stateCount; ++srcIndex)
                _srcToAnyTransitions[srcIndex] = new Transition(this, EnumTraits<T>.Values[srcIndex], null);

            for (var dstIndex = 0; dstIndex < stateCount; ++dstIndex)
                _anyToDstTransitions[dstIndex] = new Transition(this, null, EnumTraits<T>.Values[dstIndex]);

            _anyToAnyTransition = new Transition(this, null, null);
        }

        #endregion Transitions
    }
}
