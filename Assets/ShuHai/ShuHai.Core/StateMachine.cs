using System;
using System.Linq;
using System.Collections.Generic;

namespace ShuHai
{
    /// <summary>
    ///     Provides states and state transition management of specified state type.
    /// </summary>
    /// <typeparam name="T">The enum type representing the state value.</typeparam>
    /// <remarks>
    ///     The core feature of the class is about state transition. Use <see cref="GetTransition" /> method to get
    ///     the state transition you concerned for. Register event handler through <see cref="Transition.StateChanging" />
    ///     or <see cref="Transition.StateChanged" /> to get event notification for state changes.
    /// </remarks>
    public class StateMachine<T>
        where T : struct, Enum
    {
        #region Constructors

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

        #endregion Constructors

        #region State

        public T State { get => _state; set => ChangeState(value, null); }

        /// <summary>
        ///     Change current state of current state machine.
        /// </summary>
        /// <param name="newState">The desired state after change.</param>
        /// <param name="args">Arguments used when triggering state change events.</param>
        /// <returns>
        ///     <see langword="true" /> if state of current state machine changed; otherwise <see langword="false" /> if
        ///     current state of current state machine is the same as desired <paramref name="newState" />.
        /// </returns>
        public bool ChangeState(T newState, params object[] args)
        {
            T src = _state, dst = newState;
            if (EqualityComparer<T>.Default.Equals(src, dst))
                return false;

            StateEvent srcEvt = GetStateEvent(src), dstEvt = GetStateEvent(dst);
            _transitings[0] = GetTransition(src, dst);
            _transitings[1] = GetTransition(src, null);
            _transitings[2] = GetTransition(null, dst);
            _transitings[3] = GetTransition(null, null);

            dstEvt.RiseExiting(src, dst);
            srcEvt.RiseEntering(src, dst, args);
            foreach (var t in _transitings)
                t.RiseStateChanging(src, dst, args);

            _state = dst;

            foreach (var t in _transitings.Reverse())
                t.RiseStateChanged(src, dst, args);
            dstEvt.RiseExited(src, dst);
            srcEvt.RiseEntered(src, dst, args);

            return true;
        }

        private T _state;

        private readonly Transition[] _transitings = new Transition[4];

        #endregion State

        #region State Events

        public sealed class StateEvent
        {
            public event Action<StateEvent, T, T, object[]> Entering;
            public event Action<StateEvent, T, T, object[]> Entered;

            public event Action<StateEvent, T, T> Exiting;
            public event Action<StateEvent, T, T> Exited;

            public T State { get; }

            internal StateEvent(T state) { State = state; }

            internal void RiseEntering(T src, T dst, object[] args) { Entering?.Invoke(this, src, dst, args); }
            internal void RiseEntered(T src, T dst, object[] args) { Entered?.Invoke(this, src, dst, args); }

            internal void RiseExiting(T src, T dst) { Exiting?.Invoke(this, src, dst); }
            internal void RiseExited(T src, T dst) { Exited?.Invoke(this, src, dst); }
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
            /// <summary>
            ///     Occurs before the state changing and determines whether to change the state.
            /// </summary>
            public event Action<Transition, T, T, object[]> StateChanging;

            /// <summary>
            ///     Occurs after the state changed.
            /// </summary>
            public event Action<Transition, T, T, object[]> StateChanged;

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

            internal void RiseStateChanging(T src, T dst, object[] args)
            {
                StateChanging?.Invoke(this, src, dst, args);
            }

            internal void RiseStateChanged(T src, T dst, object[] args) { StateChanged?.Invoke(this, src, dst, args); }
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