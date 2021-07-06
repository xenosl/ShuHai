using System;
using UnityEngine;

namespace ShuHai.NOctree.Unity
{
    public class NOctCell
    {
        public float Size { get; }

        /// <summary>
        ///     初始化一个N阶八叉树根节点。
        /// </summary>
        /// <param name="n">子节点分割参数，子节点数量为 8^<paramref name="n" />。</param>
        /// <param name="position">该节点的位置。</param>
        /// <param name="size">该节点的边长。</param>
        public NOctCell(int n, Vector3 position, float size)
        {
            _node = new NOctNode(n);
            Position = position;
            Size = size;
            LocalPosition = Vector3.zero;
        }

        private NOctCell(NOctNode node)
        {
            _node = node;

            var p = node;
            while (p != null)
            {
                p = p.Parent;
            }
        }

        private readonly NOctNode _node;

        #region Position

        public Vector3 Position { get; }

        public Vector3 LocalPosition { get; }

        private static Vector3 PositionIndexToPosition(Vector3Int index, float cellSize)
        {
            var hs = cellSize / 2;
            var x = index.x * cellSize + hs;
            var y = index.y * cellSize + hs;
            var z = index.z * cellSize + hs;
            return new Vector3(x, y, z);
        }

        #endregion Position

        #region Hierarchy

        public bool IsRoot => _node.IsRoot;

        public NOctCell Parent
        {
            get
            {
                var p = _node.Parent;
                if (p == null)
                    return null;

                if (p.UserData == null)
                    p.UserData = new NOctCell(p);
                return (NOctCell)p.UserData;
            }
        }

        public bool IsLeaf => _node.IsLeaf;

        public NOctCell this[Vector3Int localPositionIndex]
        {
            get
            {
                //var index = PositionToLocalIndex(localPositionIndex);
                //var (cc, created) = _node.GetOrCreateChild(index.index0, index.index1, index.index2);
                //if (created)
                //    cc.UserData = new NOctCell(cc);
                //return (NOctCell)cc.UserData;
                throw new NotImplementedException();
            }
        }

        public NOctCell FindRoot()
        {
            var root = this;
            var p = Parent;
            while (p != null)
            {
                root = p;
                p = p.Parent;
            }
            return root;
        }

        #endregion Hierarchy

        #region Utilities

        /// <summary>
        ///     将用于<see cref="NOctNode" />的以0为起始值的索引转换为以0为中间值的索引以便用于实际位置计算。
        /// </summary>
        private Vector3Int LocalToPositionIndex(int index0, int index1, int index2)
        {
            var hc = _node.DimensionalChildCapacity / 2;
            return new Vector3Int(index0 - hc, index1 - hc, index2 - hc);
        }

        /// <summary>
        ///     将用于实际位置计算的以0为中间值的索引转换为用于<see cref="NOctNode" />获取子节点的以0为起始值的索引。
        /// </summary>
        private (int index0, int index1, int index2) PositionToLocalIndex(Vector3Int centerIndex)
        {
            var hc = _node.DimensionalChildCapacity / 2;
            return (centerIndex.x + hc, centerIndex.y + hc, centerIndex.z + hc);
        }

        #endregion Utilities
    }
}
