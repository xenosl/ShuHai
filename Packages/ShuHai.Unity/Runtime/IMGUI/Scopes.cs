using System;
using UnityEngine;

namespace ShuHai.Unity.IMGUI
{
    public struct GUIEnabledScope : IDisposable
    {
        public bool Disposed { get; private set; }

        public bool OldEnabled { get; }

        public GUIEnabledScope(bool enabled)
        {
            Disposed = false;

            OldEnabled = GUI.enabled;
            GUI.enabled = enabled;
        }

        public void Dispose()
        {
            if (Disposed)
                return;

            GUI.enabled = OldEnabled;

            Disposed = true;
        }
    }
}
