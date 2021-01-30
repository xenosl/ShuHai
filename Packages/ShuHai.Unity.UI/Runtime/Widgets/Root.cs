using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ShuHai.Unity.UI.Widgets
{
    [AddComponentMenu(MenuInfo.Path + "Root")]
    [RequireComponent(typeof(Canvas))]
    [DisallowMultipleComponent]
    public class Root : MonoBehaviour
    {
        public bool KeepAliveOnLoad = true;

        #region Widgets

        public T CreateWidget<T>(Transform parent = null, bool active = true)
            where T : Widget
        {
            return (T)CreateWidget(typeof(T), parent, active);
        }

        public Widget CreateWidget(Type type, Transform parent, bool active = true)
        {
            Ensure.Argument.NotNull(type, nameof(type));
            Ensure.Argument.Is<Widget>(type, nameof(type));

            if (!parent)
                parent = transform;

            var prefab = EnsurePrefab(type);
            var widget = Instantiate(prefab, parent, false);
            SetupWidget(widget, active);
            return widget;
        }

        private static void SetupWidget(Widget widget, bool active)
        {
            widget.name = widget.name.RemoveTail("(Clone)".Length);
            widget.transform.SetAsLastSibling();
            widget.gameObject.SetActive(active);
        }

        #region Panels

        public T OpenPanel<T>()
            where T : Panel
        {
            return (T)OpenPanelImpl(typeof(T));
        }

        public Panel OpenPanel(Type type)
        {
            Ensure.Argument.NotNull(type, nameof(type));
            Ensure.Argument.Is<Panel>(type, nameof(type));
            return OpenPanelImpl(type);
        }

        private Panel OpenPanelImpl(Type type) { return GetOrCreatePanelImpl(type, true); }

        public void ClosePanel<T>()
            where T : Panel
        {
            ClosePanelImpl(typeof(T));
        }

        public void ClosePanel(Type type)
        {
            Ensure.Argument.NotNull(type, nameof(type));
            Ensure.Argument.Is<Panel>(type, nameof(type));
            ClosePanelImpl(type);
        }

        public void ClosePanels()
        {
            foreach (var panel in _panels.Values.ToArray())
                ClosePanelImpl(panel.GetType());
        }

        private void ClosePanelImpl(Type type)
        {
            if (_panels.TryGetValue(type, out var panel))
                panel.Active = false;
        }

        public T GetOrCreatePanel<T>(bool? active = false)
            where T : Panel
        {
            return (T)GetOrCreatePanelImpl(typeof(T), active);
        }

        public Panel GetOrCreatePanel(Type type, bool? active = false)
        {
            Ensure.Argument.NotNull(type, nameof(type));
            Ensure.Argument.Is<Panel>(type, nameof(type));
            return GetOrCreatePanelImpl(type, active);
        }

        private Panel GetOrCreatePanelImpl(Type type, bool? active)
        {
            if (_panels.TryGetValue(type, out var panel))
                return panel;

            panel = (Panel)CreateWidget(type, transform, active ?? false);
            _panels.Add(type, panel);
            return panel;
        }

        private readonly Dictionary<Type, Panel> _panels = new Dictionary<Type, Panel>();

        #endregion Panels

        #endregion Widgets

        #region Prefabs

        [SerializeField]
        private List<Widget> _prefabs = new List<Widget>();

        private Dictionary<Type, Widget> _prefabDict;

        private void InitializePrefabs() { _prefabDict = _prefabs.ToDictionary(w => w.GetType()); }

        private void DeinitializePrefabs() { _prefabDict = null; }

        private T EnsurePrefab<T>()
            where T : Widget
        {
            var type = typeof(T);
            var prefab = EnsurePrefab(type);
            var prefabT = prefab as T;
            if (!prefabT)
                throw new InvalidOperationException($"'{type.Name}' is not a panel.");
            return prefabT;
        }

        private Widget EnsurePrefab(Type type)
        {
            if (!_prefabDict.TryGetValue(type, out var prefab))
                throw new InvalidOperationException($"Prefab '{type.Name}' not found.");
            return prefab;
        }

        #endregion Prefabs

        #region MonoBehaviour Messages

        protected virtual void Awake()
        {
            InitializePrefabs();

            if (KeepAliveOnLoad)
                DontDestroyOnLoad(gameObject);
        }

        protected virtual void OnDestroy() { DeinitializePrefabs(); }

        #endregion MonoBehaviour Messages
    }
}
