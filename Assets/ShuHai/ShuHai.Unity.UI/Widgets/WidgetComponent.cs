using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ShuHai.Unity.UI
{
    [RequireComponent(typeof(Widget))]
    public abstract class WidgetComponent<T> : UIBehaviour, IWidgetComponent<T>
        where T : Widget
    {
        public RectTransform RectTransform => Widget.RectTransform;

        protected WidgetComponent() { _widget = new Lazy<T>(EnsureWidget); }

        #region Widget

        public T Widget => _widget.Value;

        private readonly Lazy<T> _widget;

        private T EnsureWidget() { return gameObject.EnsureComponent<T>(); }

        Widget IWidgetComponent.Widget => Widget;

        #endregion Widget
    }
}