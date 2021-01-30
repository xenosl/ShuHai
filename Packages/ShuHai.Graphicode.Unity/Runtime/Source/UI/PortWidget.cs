using System;
using System.Collections.Generic;
using ShuHai.Unity;
using ShuHai.Unity.UI.Widgets;
using UnityEngine;
using UnityEngine.UI;

namespace ShuHai.Graphicode.Unity.UI
{
    public class PortWidget : Widget
    {
        public UnitPanel Owner { get; private set; }

        public string Name
        {
            get => _port != null ? _port.Name : name;
            set
            {
                if (_port != null)
                    _port.Name = value;
                name = value;
                NameText.text = value;
            }
        }

        protected PortWidget()
        {
            _nameText = new Lazy<Text>(FindNameText);

            _iconRect = new Lazy<RectTransform>(FindIconRect);
            _iconSelectable = new Lazy<Selectable>(FindIconSelectable);
        }

        #region Binding Port

        public IPort Port
        {
            get => _port;
            set
            {
                if (value == _port)
                    return;

                if (_port != null)
                    UnbindPort();
                if (value != null)
                    BindPort(value);
            }
        }

        private IPort _port;

        private void BindPort(IPort port)
        {
            _port = port;
            _boundInstances.Add(_port, this);

            Owner = UnitPanel.BoundInstances.GetValue(_port.Owner);
            transform.SetParent(Owner.SelectPortRect(_port), false);

            Name = _port.Name;
        }

        private void UnbindPort()
        {
            Name = GetType().Name;

            transform.SetParent(null);
            Owner = null;

            _boundInstances.Remove(_port);
            _port = null;
        }

        #region Port To Widget

        public static IReadOnlyDictionary<IPort, PortWidget> BoundInstances => _boundInstances;

        private static readonly Dictionary<IPort, PortWidget> _boundInstances = new Dictionary<IPort, PortWidget>();

        #endregion Port To Widget

        #endregion Binding Port

        #region Child Components

        public Vector3 IconPosition => IconRect.position;

        private Text NameText => _nameText.Value;

        private RectTransform IconRect => _iconRect.Value;
        private Selectable IconSelectable => _iconSelectable.Value;
        private readonly Lazy<Text> _nameText;

        private readonly Lazy<RectTransform> _iconRect;
        private readonly Lazy<Selectable> _iconSelectable;

        private Text FindNameText() { return gameObject.FindComponentInChild<Text>("Name"); }

        private RectTransform FindIconRect() { return gameObject.FindComponentInChild<RectTransform>("Icon"); }
        private Selectable FindIconSelectable() { return IconRect.GetComponent<Selectable>(); }

        #endregion Child Components
    }
}
