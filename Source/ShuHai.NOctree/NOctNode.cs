using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace ShuHai.NOctree
{
    /// <summary>
    ///     表示一个可向上和向下扩展的N阶八叉树的节点。N阶八叉树表示子节点为8^N个的八叉树。
    /// </summary>
    /// <remarks>
    ///     由于所有节点只有在需要时才会创建，只要有意控制每个节点的子节点的数量（由<see cref="N"/>控制）即可在表示
    ///     无限大空间时有效控制内存大小，也就是控制用于表示子节点的数组（<see cref="NOctNode._children" />）的大小。
    /// </remarks>
    public class NOctNode : IReadOnlyCollection<NOctNode>
    {
        public object UserData { get; set; }

        #region Constructors

        /// <summary>
        ///     初始化一个可容纳 8^<paramref name="n" /> 个子节点的根节点。
        /// </summary>
        /// <param name="n">子节点分割参数，子节点数量为 8^<paramref name="n" />。</param>
        public NOctNode(int n = 0)
        {
            if (n < 0)
                throw new ArgumentException("Zero or positive value expected.", nameof(n));

            N = n;
            MaxChildCount = CalculateChildCount(n);
            SegmentCount = CalculateSegmentCount(MaxChildCount);
            _children = new NOctNode[SegmentCount, SegmentCount, SegmentCount];
        }

        #endregion Constructors

        #region Hierarchy

        /// <summary>
        ///     当前节点在父节点中索引。
        /// </summary>
        public NOctIndex Index { get; private set; }

        public bool IsChildOf(NOctNode node)
        {
            Ensure.Argument.NotNull(node, nameof(node));

            var p = Parent;
            while (p != null)
            {
                if (p == node)
                    return true;
                p = p.Parent;
            }
            return false;
        }

        public bool IsParentOf(NOctNode node)
        {
            Ensure.Argument.NotNull(node, nameof(node));

            return node.IsChildOf(this);
        }

        #region Parents

        public bool IsRoot => Parent == null;

        public NOctNode Parent { get; private set; }

        public NOctNode FindRoot()
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

        #endregion Parents

        #region Children

        /// <summary>
        ///     决定最大子节点数量的参数：最大子节点数量为 8^<see cref="N" />。
        /// </summary>
        public int N { get; }

        /// <summary>
        ///     由当前节点所构成的立方体在每个边缘上的分段数量（同时也是每个维度上的子节点数量）。
        /// </summary>
        public int SegmentCount { get; }

        /// <summary>
        ///     是否是叶子节点，即当前节点是否不存在子节点。
        /// </summary>
        public bool IsLeaf => MaxChildCount == 0;

        /// <summary>
        ///     当前节点最大可容纳的子节点数。
        /// </summary>
        public int MaxChildCount { get; }

        /// <summary>
        ///     当前节点拥有的子节点数。
        /// </summary>
        public int ChildCount => _childSet.Count;

        /// <summary>
        ///     一个可遍历所有子节点的迭代器。
        /// </summary>
        public IEnumerable<NOctNode> Children => _childSet;

        public NOctNode this[int index0, int index1, int index2] => _children[index0, index1, index2];

        public NOctNode this[NOctIndex index] => _children[index.I0, index.I1, index.I2];

        public void SetChild(NOctIndex index, NOctNode child)
        {
            if (!index.IsValid(SegmentCount))
                throw new ArgumentOutOfRangeException(nameof(index));
            SetChild(index.I0, index.I1, index.I2, child);
        }

        public void SetChild(int index0, int index1, int index2, NOctNode child)
        {
            Ensure.Argument.NotNull(index0, nameof(index0));
            Ensure.Argument.NotNull(index1, nameof(index1));
            Ensure.Argument.NotNull(index2, nameof(index2));

            var oldChild = _children[index0, index1, index2];
            if (oldChild == child)
                return;

            if (oldChild != null)
                TakeChild(oldChild.Index);

            _children[index0, index1, index2] = child;

            if (child != null)
                PutChild(index0, index1, index2, child);
        }

        private NOctNode TakeChild(NOctIndex index) { return TakeChild(index.I0, index.I1, index.I2); }

        private NOctNode TakeChild(int index0, int index1, int index2)
        {
            var child = _children[index0, index1, index2];
            if (child != null)
            {
                bool removed = _childSet.Remove(child);
                Debug.Assert(removed);
                _children[index0, index1, index2] = null;

                child.Parent = null;
                child.Index = NOctIndex.Zero;
            }
            return child;
        }

        private void PutChild(int index0, int index1, int index2, NOctNode child)
        {
            Debug.Assert(_children[index0, index1, index2] == null);
            Debug.Assert(child.Parent == null);

            child.Index = new NOctIndex(index0, index1, index2);
            child.Parent = this;

            _children[index0, index1, index2] = child;
            bool added = _childSet.Add(child);
            Debug.Assert(added);
        }

        public bool IsChildExisted(int index0, int index1, int index2)
        {
            return _children[index0, index1, index2] != null;
        }

        private readonly NOctNode[,,] _children;
        private readonly HashSet<NOctNode> _childSet = new HashSet<NOctNode>();

        #endregion Children

        #endregion Hierarchy

        #region Utilities

        /// <summary>
        ///     计算：假如当前子节点为指定父节点的直接子节点（即中间没有其他父节点），那么指定父节点的N值应该是多少。
        /// </summary>
        public int NInParent(NOctNode parent)
        {
            Ensure.Argument.NotNull(parent, nameof(parent));
            if (IsRoot)
                throw new InvalidOperationException("Any parent expected.");

            int n = 0;
            var p = Parent;
            while (p != null)
            {
                n += p.N;
                if (p == parent)
                    break;
                p = p.Parent;
            }
            if (p == null)
                throw new ArgumentException("Parent node expected.", nameof(parent));

            return n;
        }

        /// <summary>
        ///     假如将指定父节点以当前节点所在的层级进行分割，当前节点在其中的位置索引。
        /// </summary>
        public void IndexInParent(NOctNode parent, out int index0, out int index1, out int index2)
        {
            Ensure.Argument.NotNull(parent, nameof(parent));
            if (IsRoot)
                throw new InvalidOperationException("Any parent expected.");

            throw new NotImplementedException();
        }

        private static int CalculateChildCount(int n) { return n > 0 ? Convert.ToInt32(Math.Pow(8, n)) : 0; }

        private static int CalculateSegmentCount(int childCount)
        {
            return childCount > 0 ? Convert.ToInt32(Math.Pow(childCount, 1.0 / 3.0)) : 0;
        }

        #endregion Utilities

        #region IReadOnlyCollection

        int IReadOnlyCollection<NOctNode>.Count => ChildCount;

        IEnumerator<NOctNode> IEnumerable<NOctNode>.GetEnumerator() { return _childSet.GetEnumerator(); }
        IEnumerator IEnumerable.GetEnumerator() { return _childSet.GetEnumerator(); }

        #endregion IReadOnlyCollection
    }
}
