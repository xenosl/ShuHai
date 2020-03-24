using System;
using ShuHai.XConverts;

namespace ShuHai.EditSystem
{
    public sealed class EditorObject : IEquatable<EditorObject>
    {
        [XConvertIgnore] public Editor Owner { get; internal set; }

        /// <summary>
        ///     Creation order of current object.
        /// </summary>
        [XConvertIgnore]
        public int Order { get; private set; }

        public object Value { get; private set; }

        internal EditorObject() { }

        internal EditorObject(int order, object value)
        {
            Order = order;
            Value = value;
        }

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