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
    ///     由于所有节点只有在需要时才会创建，只要有意控制每个节点的子节点的数量（由<see cref="N" />控制）即可在表示
    ///     无限大空间时有效控制内存大小，也就是控制用于表示子节点的数组（<see cref="NOctNode._children" />）中的内容多少。
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
                throw new ArgumentException("Zero or positive value of N expected.", nameof(n));

            N = n;
            ChildCapacity = EvaluateChildCapacity(n);
            DimensionalChildCapacity = EvaluateDimensionalChildCapacity(ChildCapacity);
            _children = new NOctNode[DimensionalChildCapacity, DimensionalChildCapacity, DimensionalChildCapacity];
        }

        #endregion Constructors

        #region Hierarchy

        /// <summary>
        ///     当前节点在父节点中索引，如果不存在父节点则值为<see cref="NOctIndex.Invalid" />。
        /// </summary>
        public NOctIndex Index { get; private set; } = NOctIndex.Invalid;

        /// <summary>
        ///     获取一个布尔值，表示当前节点是否是指定节点的子节点。
        /// </summary>
        /// <param name="node">指定的父节点。</param>
        /// <returns>一个表示当前节点是否是指定节点的子节点布尔值。</returns>
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

        /// <summary>
        ///     获取一个布尔值，表示当前节点是否是指定节点的父节点。
        /// </summary>
        /// <param name="node">指定的子节点。</param>
        /// <returns>一个表示当前节点是否是指定节点的父节点布尔值。</returns>
        public bool IsParentOf(NOctNode node)
        {
            Ensure.Argument.NotNull(node, nameof(node));

            return node.IsChildOf(this);
        }

        /// <summary>
        ///     以当前节点为根节点，计算指定子节点的深度。
        /// </summary>
        /// <param name="child">需要计算的深度的子节点，该节点必须是当前节点的子节点。</param>
        /// <returns>指定节点以当前节点为根节点时的深度值。</returns>
        public int DepthOf(NOctNode child)
        {
            if (child == null)
                throw new ArgumentNullException(nameof(child));

            int depth = 0;
            var p = child;
            while (p != null)
            {
                if (p == this)
                    break;
                depth++;
                p = p.Parent;
            }
            if (p == null)
                throw new ArgumentException("The specified node is not child of current node.", nameof(child));
            return depth;
        }

        /// <summary>
        ///     以当前节点为根节点，计算指定子节点所在的阶数。
        /// </summary>
        /// <param name="child">需要计算的深度的子节点，该节点必须是当前节点的子节点。</param>
        /// <returns>指定节点以当前节点为根节点时的阶数值。</returns>
        public int RankOf(NOctNode child)
        {
            if (child == null)
                throw new ArgumentNullException(nameof(child));

            var n = 0;
            var p = child.Parent;
            while (p != null)
            {
                n += p.N;
                if (p == this)
                    break;
                p = p.Parent;
            }
            if (p == null)
                throw new ArgumentException("The specified node is not child of current node.", nameof(child));
            return n;
        }

        /// <summary>
        ///     计算指定子节点在没有中间子节点（但合并中间子节点的阶数）的情况下在当前节点中的索引。
        /// </summary>
        public NOctIndex IndexOf(NOctNode child)
        {
            if (child == null)
                throw new ArgumentNullException(nameof(child));
            if (IsLeaf)
                throw new InvalidOperationException("Unable to evaluate child index from a leaf node.");

            var index = child.Index;
            var c = child;
            var p = c.Parent;
            while (p != null)
            {
                if (p == this)
                    break;

                var n = p.RankOf(child);
                var dc = EvaluateDimensionalChildCapacity(EvaluateChildCapacity(n));
                var i = IndexMultiply(p.Index, dc);
                index = IndexPlus(index, i);

                c = p;
                p = c.Parent;
            }

            if (p == null)
                throw new ArgumentException("The specified node is not child of current node.", nameof(child));

            return index;
        }

        private static NOctIndex IndexPlus(NOctIndex l, NOctIndex r)
        {
            return new NOctIndex(l.I0 + r.I0, l.I1 + r.I1, l.I2 + r.I2);
        }

        private static NOctIndex IndexMultiply(NOctIndex index, int scaler)
        {
            return new NOctIndex(index.I0 * scaler, index.I1 * scaler, index.I2 * scaler);
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
        ///     当前节点在单个维度上的最大子节点数。
        /// </summary>
        public int DimensionalChildCapacity { get; }

        /// <summary>
        ///     是否是叶子节点，即当前节点是否不可容纳子节点。
        /// </summary>
        public bool IsLeaf => _childSet.Count == 0;

        /// <summary>
        ///     当前节点最大可容纳的子节点数。
        /// </summary>
        public int ChildCapacity { get; }

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
            if (!index.IsValid(DimensionalChildCapacity))
                throw new ArgumentOutOfRangeException(nameof(index));
            SetChild(index.I0, index.I1, index.I2, child);
        }

        public void SetChild(int index0, int index1, int index2, NOctNode child)
        {
            Ensure.Argument.ValidIndex(index0, DimensionalChildCapacity, nameof(index0));
            Ensure.Argument.ValidIndex(index1, DimensionalChildCapacity, nameof(index1));
            Ensure.Argument.ValidIndex(index2, DimensionalChildCapacity, nameof(index2));

            var oldChild = _children[index0, index1, index2];
            if (oldChild == child)
                return;

            var otherParent = child?.Parent;
            otherParent?.TakeChild(child);

            if (oldChild != null)
                TakeChild(index0, index1, index2);
            if (child != null)
                PutChild(index0, index1, index2, child);
        }

        private NOctNode TakeChild(NOctNode child)
        {
            Debug.Assert(child.Parent == this);
            return TakeChild(child.Index);
        }

        private NOctNode TakeChild(NOctIndex index) { return TakeChild(index.I0, index.I1, index.I2); }

        private NOctNode TakeChild(int index0, int index1, int index2)
        {
            var child = _children[index0, index1, index2];
            Debug.Assert(child != null);

            var removed = _childSet.Remove(child);
            Debug.Assert(removed);
            _children[index0, index1, index2] = null;

            child.Parent = null;
            child.Index = NOctIndex.Invalid;

            return child;
        }

        private void PutChild(int index0, int index1, int index2, NOctNode child)
        {
            Debug.Assert(_children[index0, index1, index2] == null);
            Debug.Assert(child != null);
            Debug.Assert(child.Parent == null);

            child.Index = new NOctIndex(index0, index1, index2);
            child.Parent = this;

            _children[index0, index1, index2] = child;
            var added = _childSet.Add(child);
            Debug.Assert(added);
        }

        public bool IsChildExisted(int index0, int index1, int index2)
        {
            return _children[index0, index1, index2] != null;
        }

        private readonly NOctNode[,,] _children;
        private readonly HashSet<NOctNode> _childSet = new HashSet<NOctNode>();

        #endregion Children

        private static int EvaluateChildCapacity(int n) { return n > 0 ? Convert.ToInt32(Math.Pow(8, n)) : 0; }

        private static int EvaluateDimensionalChildCapacity(int childCapacity)
        {
            return childCapacity > 0 ? Convert.ToInt32(Math.Pow(childCapacity, 1.0 / 3.0)) : 0;
        }

        #endregion Hierarchy

        #region IReadOnlyCollection

        int IReadOnlyCollection<NOctNode>.Count => ChildCount;

        IEnumerator<NOctNode> IEnumerable<NOctNode>.GetEnumerator() { return _childSet.GetEnumerator(); }
        IEnumerator IEnumerable.GetEnumerator() { return _childSet.GetEnumerator(); }

        #endregion IReadOnlyCollection
    }
}
