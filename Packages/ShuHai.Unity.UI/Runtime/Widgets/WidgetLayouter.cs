using UnityEngine;

namespace ShuHai.Unity.UI.Widgets
{
    [ExecuteAlways]
    public abstract class WidgetLayouter<T> : WidgetComponent<T>, IWidgetLayouter
        where T : Widget
    {
        #region Layout Build

        public bool Dirty { get; private set; } = true;

        public void MarkDirty() { Dirty = true; }

        protected abstract void RebuildLayout();

        void IWidgetLayouter.RebuildLayout() { RebuildLayout(); }

        private void RebuildLayoutIfDirty()
        {
            if (!Dirty)
                return;
            RebuildLayout();
            Dirty = false;
        }

        #endregion Layout Build

        #region Unity Events

        protected virtual void OnTransformChildrenChanged() { MarkDirty(); }

        protected virtual void Update() { RebuildLayoutIfDirty(); }

        #endregion Unity Events
    }
}
