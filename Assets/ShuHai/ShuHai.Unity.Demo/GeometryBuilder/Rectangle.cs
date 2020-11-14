using System;
using System.Linq;
using UnityEngine;

namespace ShuHai.Unity.Demo
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(MeshFilter))]
    [ExecuteAlways]
    public class Rectangle : MonoBehaviour
    {
        public Vector2 Size = Vector2.one;

        public AxisPlane Plane = AxisPlane.XY;

        protected Rectangle() { _meshFilter = new Lazy<MeshFilter>(EnsureMeshFilter); }

        #region Mesh

        private Mesh _mesh;
        private bool _meshDirty;

        private void MarkMeshDirty() { _meshDirty = true; }

        private void RebuildMeshIfDirty()
        {
            if (_meshDirty)
                RebuildMesh();
            _meshDirty = false;
        }

        private void RebuildMesh()
        {
            if (Size.sqrMagnitude < Primitives.DefaultFloatTolerance)
                return;

            if (!_mesh)
                _mesh = new Mesh();

            var meshData = GeometryBuilder.BuildRectangle(Size, Plane);
            _mesh.vertices = meshData.vertices.Select(v => v.Position).ToArray();
            _mesh.uv = meshData.vertices.Select(v => v.Texcoord).ToArray();
            _mesh.triangles = meshData.triangles.SelectMany(t => t.ToArray()).ToArray();
            _mesh.RecalculateNormals();

            MeshFilter.sharedMesh = _mesh;
        }

        private void DestroyMesh() { UnityObjectUtil.Destroy(ref _mesh); }

        private MeshFilter MeshFilter => _meshFilter.Value;
        private readonly Lazy<MeshFilter> _meshFilter;

        private MeshFilter EnsureMeshFilter() { return gameObject.EnsureComponent<MeshFilter>(); }

        #endregion Mesh

        #region Unity Events

        private void OnDestroy() { DestroyMesh(); }

        private void Update() { RebuildMeshIfDirty(); }

#if UNITY_EDITOR
        private void OnValidate() { MarkMeshDirty(); }

        private void Reset() { MarkMeshDirty(); }
#endif // UNITY_EDITOR

        #endregion Unity Events
    }
}
