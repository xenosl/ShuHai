using System;

namespace ShuHai
{
    public static class StateMachineExtensions
    {
        public static void RegisterChangeEvents<T>(this StateMachine<T> sm, T state,
            Action<StateMachine<T>.Transition, T, T, object[]> entered,
            Action<StateMachine<T>.Transition, T, T, object[]> exiting)
            where T : struct, Enum
        {
            RegisterChangeEvents(sm, state, null, entered, exiting, null);
        }

        public static void UnregisterChangeEvents<T>(this StateMachine<T> sm, T state,
            Action<StateMachine<T>.Transition, T, T, object[]> entered,
            Action<StateMachine<T>.Transition, T, T, object[]> exiting)
            where T : struct, Enum
        {
            UnregisterChangeEvents(sm, state, null, entered, exiting, null);
        }

        public static void RegisterChangeEvents<T>(this StateMachine<T> sm, T state,
            Action<StateMachine<T>.Transition, T, T, object[]> entering,
            Action<StateMachine<T>.Transition, T, T, object[]> entered,
            Action<StateMachine<T>.Transition, T, T, object[]> exiting,
            Action<StateMachine<T>.Transition, T, T, object[]> exited)
            where T : struct, Enum
        {
            if (entering != null)
                sm.GetTransition(null, state).StateChanging += entering;
            if (entered != null)
                sm.GetTransition(null, state).StateChanged += entered;
            if (exiting != null)
                sm.GetTransition(state, null).StateChanging += exiting;
            if (exited != null)
                sm.GetTransition(state, null).StateChanged += exited;
        }

        public static void UnregisterChangeEvents<T>(this StateMachine<T> sm, T state,
            Action<StateMachine<T>.Transition, T, T, object[]> entering,
            Action<StateMachine<T>.Transition, T, T, object[]> entered,
            Action<StateMachine<T>.Transition, T, T, object[]> exiting,
            Action<StateMachine<T>.Transition, T, T, object[]> exited)
            where T : struct, Enum
        {
            if (exited != null)
                sm.GetTransition(state, null).StateChanged -= exited;
            if (exiting != null)
                sm.GetTransition(state, null).StateChanging -= exiting;
            if (entered != null)
                sm.GetTransition(null, state).StateChanged -= entered;
            if (entering != null)
                sm.GetTransition(null, state).StateChanging -= entering;
        }
    }
}
