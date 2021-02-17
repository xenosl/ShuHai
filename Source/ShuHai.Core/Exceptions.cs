using System;

namespace ShuHai
{
    public class EnumOutOfRangeException : Exception
    {
        public Enum Enum { get; }

        public override string Message
        {
            get
            {
                string msg = base.Message;
                return Enum == null ? msg : $"{msg}{Environment.NewLine}Enum type: {Enum}.";
            }
        }

        public EnumOutOfRangeException(Enum @enum, string message = "Enum value out of range.")
            : base(message)
        {
            Enum = @enum;
        }

        public EnumOutOfRangeException(string message, Exception innerException) : base(message, innerException) { }
    }

    public class MissingAttributeException : Exception
    {
        public Type AttributeType { get; }

        public override string Message
        {
            get
            {
                var msg = base.Message;
                return AttributeType == null ? msg : $"{msg}{Environment.NewLine}Attribute type: {AttributeType}.";
            }
        }

        public MissingAttributeException(string message = "Attribute is missing.", Exception innerException = null)
            : this(null, message, innerException) { }

        public MissingAttributeException(Type attributeType,
            string message = "Attribute is missing.", Exception innerException = null)
            : base(message, innerException)
        {
            AttributeType = attributeType;
        }
    }

    public class InvalidReferenceException : Exception
    {
        public InvalidReferenceException() : this("Object reference is invalid.") { }
        public InvalidReferenceException(string message) : base(message) { }
        public InvalidReferenceException(string message, Exception innerException) : base(message, innerException) { }
    }

    public class InvalidConfigException : Exception
    {
        public InvalidConfigException() : this("Configuration is invalid.") { }
        public InvalidConfigException(string message) : base(message) { }
        public InvalidConfigException(string message, Exception innerException) : base(message, innerException) { }
    }

    public class DuplicateInstanceException : Exception
    {
        public Type Type { get; }

        public override string Message
        {
            get
            {
                var msg = base.Message;
                return Type == null ? msg : $"{msg}{Environment.NewLine}Type: {Type}.";
            }
        }

        public DuplicateInstanceException(string message = "Duplicate instance.", Exception innerException = null)
            : this(null, message, innerException) { }

        public DuplicateInstanceException(Type type,
            string message = "Duplicate instance.", Exception innerException = null)
            : base(message, innerException)
        {
            Type = type;
        }
    }
}
