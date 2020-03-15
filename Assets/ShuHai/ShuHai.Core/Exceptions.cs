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

        public EnumOutOfRangeException(Enum @enum) : this(@enum, string.Empty) { }

        public EnumOutOfRangeException(Enum @enum, string message)
            : base(message)
        {
            Enum = @enum;
        }

        public EnumOutOfRangeException(string message, Exception innerException) : base(message, innerException) { }
    }

    public class MissingAttributeException : Exception
    {
        public MissingAttributeException() { }
        public MissingAttributeException(string message) : base(message) { }
        public MissingAttributeException(string message, Exception innerException) : base(message, innerException) { }
    }

    public class InvalidReferenceException : Exception
    {
        public InvalidReferenceException() { }
        public InvalidReferenceException(string message) : base(message) { }
        public InvalidReferenceException(string message, Exception innerException) : base(message, innerException) { }
    }

    public class InvalidConfigException : Exception
    {
        public InvalidConfigException() { }
        public InvalidConfigException(string message) : base(message) { }
        public InvalidConfigException(string message, Exception innerException) : base(message, innerException) { }
    }
}