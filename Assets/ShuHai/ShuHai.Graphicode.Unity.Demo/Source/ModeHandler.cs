using System;
using UnityEngine;

namespace ShuHai.Graphicode.Unity.Demo
{
    public abstract class ModeHandler : MonoBehaviour
    {
        public abstract DemoMode Mode { get; }

        public bool Initialized { get; private set; }

        public void Initialize()
        {
            if (Initialized)
                throw new InvalidOperationException("Duplicate initialization.");

            InitializeImpl();

            Initialized = true;
        }

        protected virtual void InitializeImpl() { }

        public void Deinitialize()
        {
            if (!Initialized)
                return;
            DeinitializeImpl();
            Initialized = false;
        }

        protected virtual void DeinitializeImpl() { }

        #region Unity Events

        protected virtual void OnEnable() { }

        protected virtual void OnDisable() { }

        protected virtual void Update() { }

        #endregion Unity Events
    }
}