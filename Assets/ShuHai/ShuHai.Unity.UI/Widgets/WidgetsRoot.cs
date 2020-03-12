using UnityEngine;
using UnityEngine.EventSystems;

namespace ShuHai.Unity.UI
{
    [RequireComponent(typeof(Canvas))]
    public class WidgetsRoot : UIBehaviour
    {
        public T CreateWidget<T>(T template, Transform parent = null)
            where T : Widget
        {
            UnityEnsure.Argument.NotNull(template, nameof(template));

            if (!parent)
                parent = transform;

            var widget = Instantiate(template, parent, false);
            widget.transform.SetAsLastSibling();
            return widget;
        }
    }
}