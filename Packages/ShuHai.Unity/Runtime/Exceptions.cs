using System;
using System.Runtime.Serialization;

namespace ShuHai.Unity
{
    [Serializable]
    public class MissingComponentException : SystemException
    {
        public override string Message => _message;

        public Type ComponentType { get; }

        public string HierarchyName { get; }

        public MissingComponentException() { }

        public MissingComponentException(string message)
            : this(string.Empty, null, message) { }

        public MissingComponentException(Type componentType, string message = null)
            : this(string.Empty, componentType, message) { }

        public MissingComponentException(string hierarchyName, Type componentType, string message = null)
            : base(message)
        {
            HierarchyName = hierarchyName;
            ComponentType = componentType;

            if (string.IsNullOrEmpty(message))
                message = "Missing component.";

            _message = message;
            if (!string.IsNullOrEmpty(hierarchyName))
                _message += $"\nLocation in hierarchy: '{HierarchyName}'.";
            if (componentType != null)
                _message += $"\nComponent type: '{ComponentType}'.";
        }

        public MissingComponentException(string message, Exception innerException)
            : base(message, innerException) { }

        protected MissingComponentException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }

        private string _message;
    }
}
