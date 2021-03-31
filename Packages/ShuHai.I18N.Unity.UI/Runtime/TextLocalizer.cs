using System;
using ShuHai.Unity;
using UnityEngine;
using UnityEngine.UI;

namespace ShuHai.I18N.Unity.UI
{
    [RequireComponent(typeof(Text))]
    public class TextLocalizer : MonoBehaviour
    {
        protected TextLocalizer() { _text = new Lazy<Text>(EnsureText); }

        #region Text Key

        public string TextKey
        {
            get => _textKey;
            set
            {
                if (value == _textKey)
                    return;
                _textKey = value;
                ApplyTextKey();
            }
        }

        [SerializeField]
        private string _textKey;

        private void ApplyTextKey() { Text.text = Language.Current.GetText(_textKey); }

        #endregion Text Key

        #region Text Component

        public Text Text => _text.Value;
        private readonly Lazy<Text> _text;

        private Text EnsureText() { return gameObject.EnsureComponent<Text>(); }

        #endregion Text Component

        #region Unity Events

        private void Start() { ApplyTextKey(); }

#if UNITY_EDITOR
        private void OnValidate() { ApplyTextKey(); }
#endif

        #endregion Unity Events
    }
}
