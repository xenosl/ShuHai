using ShuHai.Unity.UI;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ShuHai.Graphicode.Unity.UI
{
    [RequireComponent(typeof(UnitPanel))]
    public class UnitPanelController : WidgetComponent<UnitPanel>, IDragHandler
    {
        private void Update()
        {
            if (EventSystem.current.currentSelectedGameObject == Widget.gameObject && Widget.Selected)
            {
                if (Input.GetKey(KeyCode.Delete))
                    Widget.Unit.Owner.RemoveUnit(Widget.Unit);
            }
        }

        void IDragHandler.OnDrag(PointerEventData eventData)
        {
            Widget.RectTransform.position += (Vector3)eventData.delta;
        }
    }
}