using System;

namespace ShuHai.Unity
{
    [AttributeUsage(AttributeTargets.Class)]
    public class RootComponentAttribute : Attribute
    {
        public int Priority { get; set; }

        public bool AutoCreate { get; set; }

        public RootComponentAttribute() : this(0) { }
        public RootComponentAttribute(int priority) { Priority = priority; }
    }
}
