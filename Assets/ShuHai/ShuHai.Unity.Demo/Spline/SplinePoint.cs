using System;
using UnityEngine;

namespace ShuHai.Unity.Demo
{
    [ExecuteInEditMode]
    public class SplinePoint : MonoBehaviour
    {
        [NonSerialized]
        public SplineView Owner;

        private void OnEnable() { Owner = gameObject.GetComponentInParent<SplineView>(); }

        private void Update()
        {
            if (transform.hasChanged)
            {
                if (Owner)
                    Owner.OnPointTransformChanged(this);
                transform.hasChanged = false;
            }
        }

        public float GizmoSize = 0.1f;

        private void OnDrawGizmos() { Gizmos.DrawSphere(transform.position, GizmoSize); }
    }
}
