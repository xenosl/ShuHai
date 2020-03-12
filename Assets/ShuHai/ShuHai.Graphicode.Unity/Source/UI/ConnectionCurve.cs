using System;
using ShuHai.Unity.UI;
using UnityEngine;

namespace ShuHai.Graphicode.Unity.UI
{
    using Curve = HermiteCurve;

    [DisallowMultipleComponent]
    [RequireComponent(typeof(Curve))]
    public class ConnectionCurve : MonoBehaviour, IGraphElementEntity
    {
        public PortWidget SourceWidget { get; private set; }
        public PortWidget DestinationWidget { get; private set; }

        protected ConnectionCurve() { _curve = new Lazy<Curve>(GetCurve); }

        #region Connection Binding

        public Connection Connection
        {
            get => _connection;
            set
            {
                if (value == _connection)
                    return;

                if (value != null && !value.IsValid)
                    value = null;

                if (_connection != null)
                    UnbindConnection();
                if (value != null)
                    BindConnection(value);
            }
        }

        private Connection _connection;

        private void BindConnection(Connection connection)
        {
            _connection = connection;

            SourceWidget = PortWidget.BoundInstances.GetValue(_connection.Source);
            DestinationWidget = PortWidget.BoundInstances.GetValue(_connection.Destination);

            SetupCurve();

            name = $"Connection({SourceWidget.Name} - {DestinationWidget.Name})";
        }

        private void UnbindConnection()
        {
            name = GetType().Name;

            ClearCurve();

            SourceWidget = null;
            DestinationWidget = null;

            _connection = null;
        }

        #endregion Connection Binding

        #region Curve

        private const float CurveExtendLength = 30;

        private void SetupCurve()
        {
            if (Curve.KeyPointCount != 4)
                Curve.ClearKeyPoints();

            Curve.AddKeyPoints(new Vector2[4]);

            SetCurveSourcePoint(SourceWidget.IconPosition);
            SetCurveDestinationPoint(DestinationWidget.IconPosition);
        }

        private void ClearCurve() { Curve.ClearKeyPoints(); }

        private void SetCurveSourcePoint(Vector2 position)
        {
            if (!SourceWidget)
                throw new InvalidOperationException();

            Curve.SetKeyPoint(0, position);
            Curve.SetKeyPoint(1, position + Vector2.right * CurveExtendLength);
        }

        private void SetCurveDestinationPoint(Vector2 position)
        {
            if (!DestinationWidget)
                throw new InvalidOperationException();

            Curve.SetKeyPoint(2, position + Vector2.left * CurveExtendLength);
            Curve.SetKeyPoint(3, position);
        }

        private void CurvePointsUpdate()
        {
            if (!SourceWidget || !DestinationWidget)
                return;

            SetCurveSourcePoint(SourceWidget.IconPosition);
            SetCurveDestinationPoint(DestinationWidget.IconPosition);
        }

        private Curve Curve => _curve.Value;

        private readonly Lazy<Curve> _curve;

        private Curve GetCurve() { return gameObject.GetComponent<Curve>(); }

        #endregion Curve

        private void Update() { CurvePointsUpdate(); }

        GraphElement IGraphElementEntity.GraphElement
        {
            get => Connection;
            set => Connection = (Connection)value;
        }
    }
}