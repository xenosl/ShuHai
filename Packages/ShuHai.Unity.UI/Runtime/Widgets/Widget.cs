using System;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ShuHai.Unity.UI.Widgets
{
    [RequireComponent(typeof(RectTransform))]
    public class Widget : UIBehaviour, ISelectHandler, IDeselectHandler
    {
        public bool Active
        {
            get => gameObject.activeSelf;
            set
            {
                if (gameObject)
                    gameObject.SetActive(value);
            }
        }

        protected Widget()
        {
            _rectTransform = new Lazy<RectTransform>(EnsureRectTransform);
            _selectionOutline = new Lazy<Outline>(GetSelectionOutline);
            _layouter = new Lazy<IWidgetLayouter>(GetLayouter);
        }

        #region Root

        public Root Root
        {
            get
            {
                if (!_root)
                {
                    _root = gameObject.GetComponentsInParent<Root>(false)
                        .FirstOrDefault(c => c.isActiveAndEnabled);
                }
                return _root;
            }
        }

        private Root _root;

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

        public bool IsSelected
        {
            get => _isSelected;
            private set
            {
                _isSelected = value;
                if (SelectionOutline)
                    SelectionOutline.enabled = _isSelected;
            }
        }

        private bool _isSelected;

        private Outline SelectionOutline => _selectionOutline.Value;

        private readonly Lazy<Outline> _selectionOutline;

        private Outline GetSelectionOutline() { return gameObject.GetComponent<Outline>(); }

        void ISelectHandler.OnSelect(BaseEventData eventData) { IsSelected = eventData.selectedObject == gameObject; }

        void IDeselectHandler.OnDeselect(BaseEventData eventData)
        {
            if (eventData.selectedObject == gameObject)
                IsSelected = false;
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

        #region Layouter

        public IWidgetLayouter Layouter => _layouter.Value;

        private readonly Lazy<IWidgetLayouter> _layouter;

        private IWidgetLayouter GetLayouter() { return gameObject.GetComponent<IWidgetLayouter>(); }

        #endregion Layouter
    }
}
