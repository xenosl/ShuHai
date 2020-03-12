using System;

namespace ShuHai.Unity
{
    [AttributeUsage(AttributeTargets.Class)]
    public class RootComponentAttribute : Attribute
    {
        public readonly int Priority;
        
        // TODO: Add LifeSpan support: Application, Scene.

        public RootComponentAttribute() : this(0) { }
        public RootComponentAttribute(int priority) { Priority = priority; }
    }
}
