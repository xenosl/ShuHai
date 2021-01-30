using System;
using System.Collections.Generic;
using System.Linq;
using ShuHai.Unity;
using ShuHai.Unity.UI;
using ShuHai.Unity.UI.Widgets;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ShuHai.Graphicode.Unity.UI
{
    [RequireComponent(typeof(PortWidget))]
    internal class PortWidgetController : WidgetComponent<PortWidget>, IBeginDragHandler, IEndDragHandler, IDragHandler
    {
        private PortWidget _connectWidget;

        private Line _connectLine;

        private void UpdateLinePoints()
        {
            _connectLine.SetPoint(0, GetLinePoint(0));
            _connectLine.SetPoint(1, GetLinePoint(1));
        }

        private Vector2 GetLinePoint(int index)
        {
            switch (index)
            {
                case 0:
                    return Widget.IconPosition;
                case 1:
                    return Input.mousePosition;
                default:
                    throw new ArgumentOutOfRangeException(nameof(index));
            }
        }

        void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
        {
            Debug.Assert(!_connectLine);

            _connectLine = Line.Create(Widget.Canvas);
            _connectLine.name = "ConnectLine";
            _connectLine.color = new Color(1, 1, 1, 0.5f);

            _connectLine.AddPoint(GetLinePoint(0));
            _connectLine.AddPoint(GetLinePoint(1));

            UpdateLinePoints();
        }

        void IEndDragHandler.OnEndDrag(PointerEventData eventData)
        {
            Debug.Assert(_connectLine);

            Destroy(_connectLine.gameObject);

            if (_connectWidget)
                Widget.Port.Connect(_connectWidget.Port);
        }

        private readonly List<RaycastResult> _raycastResultsOnDrag = new List<RaycastResult>();

        void IDragHandler.OnDrag(PointerEventData eventData)
        {
            _raycastResultsOnDrag.Clear();
            Widget.Raycaster.Raycast(eventData, _raycastResultsOnDrag);

            _connectWidget = _raycastResultsOnDrag
                .Select(r => r.gameObject)
                .Where(o => !o.IsChildOf(Widget.Owner.gameObject))
                .Select(o => o.GetComponentInParent<PortWidget>())
                .FirstOrDefault(w => w);

            UpdateLinePoints();
        }
    }
}
