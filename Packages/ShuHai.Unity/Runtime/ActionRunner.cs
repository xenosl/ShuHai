using System;
using System.Collections.Concurrent;
using UnityEngine;
using UnityEngine.LowLevel;

namespace ShuHai.Unity
{
    public static class ActionRunner
    {
        #region Actions

        public static IProducerConsumerCollection<Action> Actions => _actions;

        private static readonly object _actionsMutex = new object();
        private static readonly ConcurrentQueue<Action> _actions = new ConcurrentQueue<Action>();

        private static void ActionsUpdate()
        {
            foreach (var action in _actions)
                action();
        }

        #endregion Actions

        #region Update

        private static readonly PlayerLoopSystem _loopSystem = new PlayerLoopSystem
        {
            type = typeof(ActionRunner),
            updateDelegate = Update
        };

        private static void Update() { ActionsUpdate(); }

        private static void SetupUpdate()
        {
            var loop = PlayerLoop.GetCurrentPlayerLoop();
            PlayerLoopSystemUtil.AddSubSystem(ref loop, _loopSystem);
            PlayerLoop.SetPlayerLoop(loop);
        }

        #endregion Update

#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
#else
        [RuntimeInitializeOnLoadMethod]
#endif
        private static void Setup() { SetupUpdate(); }
    }
}
