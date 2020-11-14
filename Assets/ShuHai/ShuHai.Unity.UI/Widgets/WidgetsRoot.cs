using UnityEngine;
using UnityEngine.EventSystems;

namespace ShuHai.Unity.UI
{
    [RequireComponent(typeof(Canvas))]
    public class WidgetsRoot : UIBehaviour
    {
        public T CreateWidget<T>(T template, Transform parent = null, bool active = true)
            where T : Widget
        {
            UnityEnsure.Argument.NotNull(template, nameof(template));

            if (!parent)
                parent = transform;

            var widget = Instantiate(template, parent, false);
            SetupWidget(widget, active);
            return widget;
        }

        public Widget CreateWidget(Widget template, Transform parent, bool active = true)
        {
            UnityEnsure.Argument.NotNull(template, nameof(template));

            if (!parent)
                parent = transform;

            var widget = Instantiate(template, parent, false);
            SetupWidget(widget, active);
            return widget;
        }

        private static void SetupWidget(Widget widget, bool active)
        {
            widget.name = widget.name.RemoveTail("(Clone)".Length);
            widget.transform.SetAsLastSibling();
            widget.gameObject.SetActive(active);
        }
    }
}