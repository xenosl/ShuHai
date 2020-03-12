using System;
using ShuHai.Unity.UI;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ShuHai.Graphicode.Unity.UI
{
    [RequireComponent(typeof(GraphPanel))]
    public class GraphPanelController : WidgetComponent<GraphPanel>, IPointerClickHandler
    {
        #region Unit Selector

        public UnitSelector UnitSelectorTemplate;

        public UnitSelector UnitSelector { get; private set; }

        private void CreateUnitSelector()
        {
            UnitSelectorTemplate.Active = false;
            UnitSelector = Instantiate(UnitSelectorTemplate, Widget.Canvas.transform);
        }

        private void EnableUnitSelector() { UnitSelector.SelectUnit += OnSelectUnit; }

        private void DisableUnitSelector() { UnitSelector.SelectUnit -= OnSelectUnit; }

        private void OnSelectUnit(Type unitType)
        {
            UnitSelector.Active = false;

            var unitPanel = Widget.CreateUnitAndPanel(unitType);
            unitPanel.RectTransform.position = Input.mousePosition;
        }

        void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
        {
            switch (eventData.button)
            {
                case PointerEventData.InputButton.Left:
                    UnitSelector.Active = false;
                    break;
                case PointerEventData.InputButton.Right:
                    UnitSelector.transform.position = eventData.position;
                    UnitSelector.Active = true;
                    UnitSelector.FocusOnSearchInput();
                    break;
                case PointerEventData.InputButton.Middle:
                    break;
                default:
                    throw new EnumOutOfRangeException(eventData.button);
            }
        }

        #endregion Unit Selector

        #region Unity Events

        protected override void Awake()
        {
            base.Awake();
            CreateUnitSelector();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            EnableUnitSelector();
        }

        protected override void OnDisable()
        {
            DisableUnitSelector();
            base.OnDisable();
        }

        #endregion Unity Events
    }
}