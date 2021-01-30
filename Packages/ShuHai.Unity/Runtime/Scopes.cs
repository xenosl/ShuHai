using System;
using UnityEngine;

namespace ShuHai.Unity
{
    public struct GizmosColorScope : IDisposable
    {
        public bool Disposed { get; private set; }

        public Color OldColor { get; }

        public GizmosColorScope(Color color)
        {
            Disposed = false;

            OldColor = Gizmos.color;
            Gizmos.color = color;
        }

        public void Dispose()
        {
            if (Disposed)
                return;

            Gizmos.color = OldColor;

            Disposed = true;
        }
    }
}
