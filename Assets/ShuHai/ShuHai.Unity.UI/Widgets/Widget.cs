using System;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ShuHai.Unity.UI
{
    [RequireComponent(typeof(RectTransform))]
    public class Widget : UIBehaviour, ISelectHandler, IDeselectHandler
    {
        public bool Active
        {
            get => gameObject.activeSelf;
            set => gameObject.SetActive(value);
        }

        protected Widget()
        {
            _rectTransform = new Lazy<RectTransform>(EnsureRectTransform);
            _selectionOutline = new Lazy<Outline>(GetSelectionOutline);
        }

        #region Root

        public WidgetsRoot Root
        {
            get
            {
                if (!_root)
                    _root = gameObject.GetComponentsInParent<WidgetsRoot>(false).FirstOrDefault(c => c.isActiveAndEnabled);
                return _root;
            }
        }

        private WidgetsRoot _root;
        
        #endregion Root

        #region Canvas

        public Canvas Canvas
        {
            get
            {
                if (!_canvas)
                    _canvas = gameObject.GetComponentsInParent<Canvas>(false).FirstOrDefault(c => c.isActiveAndEnabled);
                return _canvas;
            }
        }

        private Canvas _canvas;

        protected override void OnCanvasHierarchyChanged()
        {
            base.OnCanvasHierarchyChanged();
            _root = null;
            _canvas = null;
            _raycaster = null;
        }

        #endregion Canvas

        #region Raycaster

        public GraphicRaycaster Raycaster
        {
            get
            {
                if (!_raycaster)
                    _raycaster = Canvas ? Canvas.GetComponent<GraphicRaycaster>() : null;
                return _raycaster;
            }
        }

        private GraphicRaycaster _raycaster;

        #endregion Raycaster

        #region Selection

        public bool Selected
        {
            get => _selected;
            private set
            {
                _selected = value;
                if (SelectionOutline)
                    SelectionOutline.enabled = _selected;
            }
        }

        private bool _selected;

        private Outline SelectionOutline => _selectionOutline.Value;

        private readonly Lazy<Outline> _selectionOutline;

        private Outline GetSelectionOutline() { return gameObject.GetComponent<Outline>(); }

        void ISelectHandler.OnSelect(BaseEventData eventData) { Selected = eventData.selectedObject == gameObject; }

        void IDeselectHandler.OnDeselect(BaseEventData eventData)
        {
            if (eventData.selectedObject == gameObject)
                Selected = false;
        }

        #endregion Selection

        #region Transform

        public RectTransform RectTransform => _rectTransform.Value;

        private readonly Lazy<RectTransform> _rectTransform;

        private RectTransform EnsureRectTransform() { return gameObject.EnsureComponent<RectTransform>(); }

        protected override void OnTransformParentChanged()
        {
            base.OnTransformParentChanged();
            _root = null;
            _canvas = null;
            _raycaster = null;
        }

        #endregion Transform
    }
}