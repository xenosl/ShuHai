using System;

namespace ShuHai.SConverts
{
    public abstract class SConverter : ISConverter
    {
        public abstract Type TargetType { get; }

        public string ToString(object value)
        {
            if (!this.CanConvert(value))
                throw new ArgumentException("Unable to convert specified value to string.", nameof(value));
            return ToStringImpl(value);
        }

        protected virtual string ToStringImpl(object value) { return value?.ToString(); }

        public object ToValue(string str)
        {
            Ensure.Argument.NotNull(str, nameof(str));
            return ToValueImpl(str);
        }

        protected abstract object ToValueImpl(string str);
    }

    public abstract class SConverter<T> : SConverter, ISConverter<T>
    {
        public sealed override Type TargetType => typeof(T);

        public virtual string ToString(T value) { return value?.ToString(); }

        protected sealed override string ToStringImpl(object value) { return ToString((T)value); }

        public new abstract T ToValue(string str);

        protected sealed override object ToValueImpl(string str) { return ToValue(str); }
    }
}