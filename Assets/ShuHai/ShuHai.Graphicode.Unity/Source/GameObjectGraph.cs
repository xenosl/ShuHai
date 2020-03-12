using System;
using System.Collections.Generic;
using ShuHai.Graphicode.Unity.UI;
using ShuHai.Graphicode.Unity.Units;
using UnityEngine;

namespace ShuHai.Graphicode.Unity
{
    [DisallowMultipleComponent]
    public class GameObjectGraph : MonoBehaviour
    {
        public Canvas Canvas;

        public readonly Graph Graph = new Graph();

        #region Graph Panel

        public GraphPanel PanelTemplate;

        public bool PanelActive
        {
            get => _panel && _panel.gameObject.activeInHierarchy;
            set => _panel.gameObject.SetActive(value);
        }

        private GraphPanel _panel;

        private void CreateGraphPanel()
        {
            if (!PanelTemplate)
                throw new InvalidOperationException("A template is required to create graph panel.");
            if (!Canvas)
                throw new InvalidOperationException("A canvas is required to contain graph panel.");

            _panel = Instantiate(PanelTemplate, Canvas.transform);
            _panel.Graph = Graph;
        }

        private void DestroyGraphPanel()
        {
            if (!_panel)
                return;

            _panel.Graph = null;
            Destroy(_panel.gameObject);
        }

        #endregion Graph Panel

        #region Elements

        #region Built-in Elements

        private MonoBehaviourUnit BehaviourUnit =>
            (MonoBehaviourUnit)(_behaviourUnitPanel ? _behaviourUnitPanel.Unit : null);

        private UnitPanel _behaviourUnitPanel;

        private void CreateBuiltinElements()
        {
            _behaviourUnitPanel = _panel.CreateUnitAndPanel(typeof(MonoBehaviourUnit));
            _behaviourUnitPanel.Name = name;
        }

        private void DestroyBuiltinElements()
        {
            _panel.DestroyUnitPanel(_behaviourUnitPanel.Unit);
            _behaviourUnitPanel = null;
        }

        #endregion Built-in Elements

        #endregion Elements

        #region Unity Events

        private void Awake()
        {
            Graph.Name = name;
            CreateGraphPanel();
            CreateBuiltinElements();

            BehaviourUnit.AwakePort.Flow();

            InstanceAwake(this);
        }

        private void OnDestroy()
        {
            InstanceDestroy(this);

            BehaviourUnit.OnDestroyPort.Flow();

            DestroyBuiltinElements();
            DestroyGraphPanel();
            Graph.Name = Graph.GetType().Name;
        }

        private void Start() { BehaviourUnit.StartPort.Flow(); }
        private void OnEnable() { BehaviourUnit.OnEnablePort.Flow(); }
        private void OnDisable() { BehaviourUnit.OnDisablePort.Flow(); }
        private void Update() { BehaviourUnit.UpdatePort.Flow(); }
        private void LateUpdate() { BehaviourUnit.LateUpdatePort.Flow(); }

        private void OnApplicationQuit()
        {
            BehaviourUnit.OnApplicationQuit.Flow();

            Destroy(gameObject);
        }

        #endregion Unity Events

        #region Instances

        public static IReadOnlyDictionary<Graph, GameObjectGraph> Instances => _instances;

        private static readonly Dictionary<Graph, GameObjectGraph>
            _instances = new Dictionary<Graph, GameObjectGraph>();

        private static void InstanceAwake(GameObjectGraph inst) { _instances.Add(inst.Graph, inst); }

        private static void InstanceDestroy(GameObjectGraph inst) { _instances.Remove(inst.Graph); }

        #endregion Instances
    }
}
