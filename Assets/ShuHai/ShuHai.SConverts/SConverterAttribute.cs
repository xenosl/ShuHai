using System;

namespace ShuHai.SConverts
{
    [AttributeUsage(AttributeTargets.Class)]
    public class SConverterAttribute : Attribute
    {
        public int Priority;

        public SConverterAttribute(int priority = 0) { Priority = priority; }
    }
}