using UnityEngine;

namespace ShuHai.Unity.UI.Widgets.Editor
{
    public class WidgetLayouterEditor<T> : UnityEditor.Editor
        where T : Widget
    {
        public WidgetLayouter<T> Target => (WidgetLayouter<T>)target;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            LayoutBuildGUI();
        }

        protected virtual void LayoutBuildGUI()
        {
            if (GUILayout.Button("Rebuild Layout"))
                Target.MarkDirty();
        }
    }
}
