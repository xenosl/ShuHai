using System;
using ShuHai.Unity;
using ShuHai.Unity.UI.Widgets;
using UnityEngine.UI;

namespace ShuHai.Graphicode.Unity.UI
{
    public class UnitListItem : Widget
    {
        public UnitList Owner { get; internal set; }

        public string Name
        {
            get => Text.text;
            private set
            {
                Text.text = value;
                name = value;
            }
        }

        public Type UnitType
        {
            get => _unitType;
            set
            {
                if (value == _unitType)
                    return;

                _unitType = value;

                Name = _unitType != null ? _unitType.FullName : GetType().Name;
            }
        }

        private Type _unitType;

        public event Action<UnitListItem> ButtonClick;

        private void OnButtonClick() { ButtonClick?.Invoke(this); }

        protected UnitListItem()
        {
            _button = new Lazy<Button>(GetButton);
            _text = new Lazy<Text>(FindText);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            Button.onClick.AddListener(OnButtonClick);
        }

        protected override void OnDisable()
        {
            Button.onClick.RemoveListener(OnButtonClick);
            base.OnDisable();
        }

        #region Child Components

        private Button Button => _button.Value;
        private Text Text => _text.Value;

        private readonly Lazy<Button> _button;
        private readonly Lazy<Text> _text;

        private Button GetButton() { return gameObject.GetComponent<Button>(); }
        private Text FindText() { return gameObject.FindComponentInChild<Text>("Text"); }

        #endregion Child Components
    }
}