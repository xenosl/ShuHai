using System;
using System.Diagnostics;
using ShuHai.XConverts;

namespace ShuHai.EditSystem
{
    public sealed class EditorObject : IEquatable<EditorObject>
    {
        /// <summary>
        ///     Creation order of current object.
        /// </summary>
        [XConvertIgnore]
        public int Order { get; private set; }

        public string Name;

        public object Value { get; }

        internal EditorObject(object value) { Value = value; }

        #region Owner

        [XConvertIgnore]
        public Editor Owner { get; private set; }

        public bool IsValid => Owner != null;

        internal void OnAddToEditor(Editor editor, int order)
        {
            Debug.Assert(Owner == null);

            Owner = editor;
            Order = order;
        }

        internal void OnRemoveFromEditor(Editor editor)
        {
            Debug.Assert(Owner == editor);

            Order = default;
            Owner = null;
        }

        #endregion Owner

        #region Comparsion

        public bool Equals(EditorObject other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(Owner, other.Owner) && Order == other.Order;
        }

        public override bool Equals(object obj)
        {
            return ReferenceEquals(this, obj) || obj is EditorObject other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Owner != null ? Owner.GetHashCode() : 0, Order.GetHashCode());
        }

        #endregion Comparsion
    }
}