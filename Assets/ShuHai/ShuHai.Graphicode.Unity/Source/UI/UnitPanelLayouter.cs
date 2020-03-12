using System;
using System.Collections.Generic;
using System.Linq;
using ShuHai.Unity;
using ShuHai.Unity.UI;
using UnityEngine;

namespace ShuHai.Graphicode.Unity.UI
{
    [RequireComponent(typeof(UnitPanel))]
    public class UnitPanelLayouter : WidgetLayouter<UnitPanel>
    {
        protected UnitPanelLayouter() { _portsRect = new Lazy<RectTransform>(FindPortsRect); }

        protected override void RebuildLayout()
        {
            var widgets = Widget.PortWidgets;
            var ih = CalculatePortsHeight(widgets.Values.Where(w => w.Port is IInputPort));
            var oh = CalculatePortsHeight(widgets.Values.Where(w => w.Port is IOutputPort));
            PortsRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Mathf.Max(ih, oh));
        }

        private static float CalculatePortsHeight(IEnumerable<PortWidget> widgets)
        {
            return widgets.Select(w => w.RectTransform.rect.height).Aggregate((s, h) => s + h);
        }

        private RectTransform PortsRect => _portsRect.Value;
        private readonly Lazy<RectTransform> _portsRect;

        private RectTransform FindPortsRect() { return gameObject.FindComponentInChild<RectTransform>("Ports"); }
    }
}