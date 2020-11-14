using System;
using UnityEngine;

namespace ShuHai.NOctree.Unity
{
    public class NOctGrid : MonoBehaviour
    {
        #region Cells

        public float LeafCellSize = 0.25f;

        public int GrowN = 3;

        public NOctCell RootCell => _rootCell;

        public NOctCell GetCell(Vector3 position) { throw new NotImplementedException(); }

        private NOctCell _rootCell;

        private void InitializeCells()
        {
            if (_rootCell != null)
                throw new InvalidOperationException("Duplicated initialization call.");
            if (GrowN < 1)
                throw new InvalidOperationException("A value greater than 0 for grow exponent expected.");

            var rootNode = new NOctNode(GrowN);
            var rootCellSize = rootNode.SegmentCount * LeafCellSize;
            _rootCell = new NOctCell(GrowN, transform.position, rootCellSize);
        }

        private void DeinitializeCells()
        {
            if (_rootCell == null)
                return;
        }

        #endregion Cells

        #region Unity Events

        private void Awake() { InitializeCells(); }

        private void OnDestroy() { DeinitializeCells(); }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if (_rootCell != null)
                DrawCellGizmo(_rootCell);
        }

        private void DrawCellGizmo(NOctCell cell) { }
#endif // UNITY_EDITOR

        #endregion Unity Events
    }
}
