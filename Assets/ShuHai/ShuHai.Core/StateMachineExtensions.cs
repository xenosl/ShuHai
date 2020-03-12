using System;

namespace ShuHai
{
    public static class StateMachineExtensions
    {
        public static void RegisterChangeEventPair<T>(this StateMachine<T> sm, T state,
            Action<StateMachine<T>.Event> entered, Action<StateMachine<T>.Event> exiting)
            where T : struct, Enum
        {
            sm.GetEvent(null, state).Changed += entered;
            sm.GetEvent(state, null).Changing += exiting;
        }

        public static void UnregisterChangeEventPair<T>(this StateMachine<T> sm, T state,
            Action<StateMachine<T>.Event> entered, Action<StateMachine<T>.Event> exiting)
            where T : struct, Enum
        {
            sm.GetEvent(state, null).Changing -= exiting;
            sm.GetEvent(null, state).Changed -= entered;
        }
    }
}
