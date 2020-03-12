using System;
using System.Collections.Generic;
using UnityEngine;

namespace ShuHai.Unity
{
    [DisallowMultipleComponent]
    public class FreeLook : MonoBehaviour
    {
        #region Steering

        [Serializable]
        public struct SteeringAxisName
        {
            public string X;
            public string Y;

            public SteeringAxisName(string x, string y)
            {
                X = x;
                Y = y;
            }
        }

        public List<SteeringAxisName> SteeringAxises = new List<SteeringAxisName>
        {
            new SteeringAxisName("Mouse X", "Mouse Y")
        };

        private void SteeringUpdate()
        {
            if (SteeringAxises.Count == 0)
                return;

            float dx = 0, dy = 0;
            foreach (var axis in SteeringAxises)
            {
                dx += Input.GetAxis(axis.X);
                dy += Input.GetAxis(axis.Y);
            }
            if (dx == 0 && dy == 0)
                return;

            dx *= SteeringMultiply;
            dy *= SteeringMultiply;

            var trans = transform;
            var rot = trans.rotation;
            rot = Quaternion.AngleAxis(dx, Vector3.up) * Quaternion.AngleAxis(-dy, trans.right) * rot;
            trans.rotation = rot;
        }

        #region Multiply

        public const float MinSteeringMultiply = 0.01f;
        public const float MaxSteeringMultiply = 100f;

        public float SteeringMultiply
        {
            get => _steeringMultiply;
            set
            {
                _steeringMultiply = value;
                ValidateSteeringMultiply();
            }
        }

        public float _steeringMultiply = 3;

        private void ValidateSteeringMultiply()
        {
            _steeringMultiply = Mathf.Clamp(_steeringMultiply, MinSteeringMultiply, MaxSteeringMultiply);
        }

        #endregion Multiply

        #endregion Steering

        #region Move

        public KeyCode MoveForwardKey = KeyCode.W;
        public KeyCode MoveBackwardKey = KeyCode.S;
        public KeyCode MoveLeftKey = KeyCode.A;
        public KeyCode MoveRightKey = KeyCode.D;

        private void MoveUpdate()
        {
            var dir = Vector2.zero;
            if (Input.GetKey(MoveForwardKey))
                dir.y += 1;
            if (Input.GetKey(MoveBackwardKey))
                dir.y -= 1;
            if (Input.GetKey(MoveLeftKey))
                dir.x -= 1;
            if (Input.GetKey(MoveRightKey))
                dir.x += 1;
            if (dir.x == 0 && dir.y == 0)
                return;

            dir.Normalize();
            var delta = MoveSpeed * dir;
            var trans = transform;
            var rot = trans.rotation;
            var deltaZ = rot * Vector3.forward * delta.y;
            trans.position += deltaZ + trans.right * delta.x;
        }

        #region Speed

        public const float MinMoveSpeed = 0.0001f;
        public const float MaxMoveSpeed = 1000000f;

        public float MoveSpeed
        {
            get => _moveSpeed;
            set
            {
                _moveSpeed = value;
                ValidateMoveSpeed();
            }
        }

        [SerializeField] private float _moveSpeed = 0.1f;

        private void ValidateMoveSpeed() { _moveSpeed = Mathf.Clamp(_moveSpeed, MinMoveSpeed, MaxMoveSpeed); }

        #endregion Speed

        #endregion Move

        #region Unity Events

        private void OnEnable() { Cursor.lockState = CursorLockMode.Locked; }

        private void OnDisable() { Cursor.lockState = CursorLockMode.None; }

        private void Update()
        {
            SteeringUpdate();
            MoveUpdate();
        }

        private void OnValidate()
        {
            ValidateSteeringMultiply();
            ValidateMoveSpeed();
        }

        #endregion Unity Events
    }
}
