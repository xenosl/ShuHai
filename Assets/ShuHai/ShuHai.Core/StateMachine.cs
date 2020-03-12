using System;
using System.Collections.Generic;

namespace ShuHai
{
    /// <summary>
    ///     Provides state and state event management of specified state type.
    /// </summary>
    /// <typeparam name="T">The enum type representing the state value.</typeparam>
    /// <remarks>
    ///     The core feature of the class is event notification. Use <see cref="StateMachine{T}.GetEvent" /> method to get
    ///     the event you concerned for, and register event handler through <see cref="StateMachine{T}.Event.Changing" />
    ///     or <see cref="StateMachine{T}.Event.Changed" />. Thus any change of the <see cref="StateMachine{T}.State" />
    ///     property would send notification(s) to your event handler.
    /// </remarks>
    public class StateMachine<T>
        where T : struct, Enum
    {
        public T State
        {
            get => _state;
            set
            {
                if (EqualityComparer<T>.Default.Equals(value, _state))
                    return;

                T src = _state, dst = value;
                NotifyStateChanging(src, dst);
                _state = dst;
                NotifyStateChanged(src, dst);
            }
        }

        private T _state;

        public StateMachine() : this(default) { }

        /// <summary>
        ///     Initialize a new state machine with specified initial state.
        /// </summary>
        /// <param name="initialState">The value of initial state.</param>
        public StateMachine(T initialState)
        {
            _state = initialState;
            InitializeEvents();
        }

        #region Events

        /// <summary>
        ///     Represents a state change event.
        /// </summary>
        public sealed class Event
        {
            /// <summary>
            ///     Occurs before the state changing.
            /// </summary>
            public event Action<Event> Changing
            {
                add => _changing.Add(value);
                remove => _changing.Remove(value);
            }

            /// <summary>
            ///     Occurs after the state changed.
            /// </summary>
            public event Action<Event> Changed
            {
                add => _changed.Add(value);
                remove => _changed.Remove(value);
            }

            /// <summary>
            ///     The source state from which the current event changes from.
            ///     A value of <see langword="null" /> indicates any state.
            /// </summary>
            public readonly T? SourceState;

            /// <summary>
            ///     The destination state to which the current event changes to.
            ///     A value of <see langword="null" /> indicates any state.
            /// </summary>
            public readonly T? DestinationState;

            /// <summary>
            ///     The owner state machine of current event.
            /// </summary>
            public readonly StateMachine<T> Owner;

            internal Event(StateMachine<T> owner, T? sourceState, T? destinationState)
            {
                Owner = owner;
                SourceState = sourceState;
                DestinationState = destinationState;
            }

            internal void RiseChanging() { Rise(_changing); }
            internal void RiseChanged() { Rise(_changed); }

            private readonly HashSet<Action<Event>> _changing = new HashSet<Action<Event>>();
            private readonly HashSet<Action<Event>> _changed = new HashSet<Action<Event>>();

            private void Rise(IEnumerable<Action<Event>> events)
            {
                foreach (var evt in events)
                    evt?.Invoke(this);
            }
        }

        /// <summary>
        ///     Get the event object representing the state change from the specified source state to destination state.
        /// </summary>
        /// <param name="sourceState">The state before the event occuring.</param>
        /// <param name="destinationState">The state after the event occured.</param>
        public Event GetEvent(T? sourceState, T? destinationState)
        {
            int srcIndex = sourceState.HasValue ? EnumTraits<T>.IndexOf(sourceState.Value) : Index.Invalid,
                dstIndex = destinationState.HasValue ? EnumTraits<T>.IndexOf(destinationState.Value) : Index.Invalid;

            if (sourceState.HasValue && destinationState.HasValue)
                return _srcToDstEvents[srcIndex, dstIndex];
            if (sourceState.HasValue) // && !destinationState.HasValue
                return _srcToAnyEvents[srcIndex];
            if (destinationState.HasValue) // && !sourceState.HasValue
                return _anyToDstEvents[dstIndex];
            return _anyToAnyEvent; // !sourceState.HasValue && !destinationState.HasValue
        }

        private readonly Event[,] _srcToDstEvents = new Event[EnumTraits<T>.ElementCount, EnumTraits<T>.ElementCount];
        private readonly Event[] _srcToAnyEvents = new Event[EnumTraits<T>.ElementCount];
        private readonly Event[] _anyToDstEvents = new Event[EnumTraits<T>.ElementCount];
        private Event _anyToAnyEvent;

        private void InitializeEvents()
        {
            int stateCount = EnumTraits<T>.ElementCount;

            for (var srcIndex = 0; srcIndex < stateCount; ++srcIndex)
            {
                var src = EnumTraits<T>.Values[srcIndex];
                for (var dstIndex = 0; dstIndex < stateCount; ++dstIndex)
                {
                    var dst = EnumTraits<T>.Values[dstIndex];
                    _srcToDstEvents[srcIndex, dstIndex] = new Event(this, src, dst);
                }
            }

            for (var srcIndex = 0; srcIndex < stateCount; ++srcIndex)
                _srcToAnyEvents[srcIndex] = new Event(this, EnumTraits<T>.Values[srcIndex], null);

            for (var dstIndex = 0; dstIndex < stateCount; ++dstIndex)
                _anyToDstEvents[dstIndex] = new Event(this, null, EnumTraits<T>.Values[dstIndex]);

            _anyToAnyEvent = new Event(this, null, null);
        }

        private void NotifyStateChanging(T src, T dst)
        {
            GetEvent(src, dst).RiseChanging();
            GetEvent(src, null).RiseChanging();
            GetEvent(null, dst).RiseChanging();
            GetEvent(null, null).RiseChanging();
        }

        private void NotifyStateChanged(T src, T dst)
        {
            GetEvent(null, null).RiseChanged();
            GetEvent(null, dst).RiseChanged();
            GetEvent(src, null).RiseChanged();
            GetEvent(src, dst).RiseChanged();
        }

        #endregion Events
    }
}
