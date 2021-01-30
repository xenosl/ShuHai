using System;

namespace ShuHai.Unity.Editor
{
    public class SerializedPropertyNotFoundException : Exception
    {
        public string PropertyName { get; }

        public override string Message
        {
            get
            {
                var msg = base.Message;
                return string.IsNullOrEmpty(PropertyName)
                    ? msg
                    : $"{msg}{Environment.NewLine}Property: {PropertyName}.";
            }
        }

        public SerializedPropertyNotFoundException(string propertyName, string message)
            : this(propertyName, message, null)
        {
            PropertyName = propertyName;
        }

        public SerializedPropertyNotFoundException(string propertyName,
            string message = "The specified property not found.", Exception innerException = null)
            : base(message, innerException)
        {
            PropertyName = propertyName;
        }
    }
}
