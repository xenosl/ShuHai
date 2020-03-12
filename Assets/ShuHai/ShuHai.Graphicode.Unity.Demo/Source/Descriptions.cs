using ShuHai.Unity;
using UnityEngine;

namespace ShuHai.Graphicode.Unity.Demo
{
    public class Descriptions : MonoBehaviour
    {
        public DemoMode ActiveRectMode
        {
            get => _activeRectMode;
            set
            {
                if (value == _activeRectMode)
                    return;

                foreach (var mode in EnumTraits<DemoMode>.Values)
                    SetActive(mode, mode == value);
                _activeRectMode = value;
            }
        }

        private DemoMode _activeRectMode;

        private RectTransform[] _rects;

        private void SetActive(DemoMode mode, bool active)
        {
            var r = GetRect(mode);
            if (r)
                r.gameObject.SetActive(active);
        }

        private RectTransform GetRect(DemoMode mode) { return _rects[(int)mode]; }

        private void Awake()
        {
            _rects = new RectTransform[EnumTraits<DemoMode>.ElementCount];
            _rects[(int)DemoMode.Inactive] = null;
            _rects[(int)DemoMode.Play] = gameObject.FindComponentInChild<RectTransform>("PlayMode");
            _rects[(int)DemoMode.Edit] = gameObject.FindComponentInChild<RectTransform>("EditMode");
        }
    }
}
