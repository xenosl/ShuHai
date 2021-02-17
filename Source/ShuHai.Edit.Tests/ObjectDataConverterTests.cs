using System;
using NUnit.Framework;

namespace ShuHai.Edit.Tests
{
    public class ObjectDataConverterTests
    {
        private abstract class TestObjectBase : IEquatable<TestObjectBase>
        {
            public string Name { get; set; }

            public TestObjectBase() : this(string.Empty) { }

            public TestObjectBase(string name) { Name = name; }

            public bool Equals(TestObjectBase other)
            {
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;
                return Name == other.Name;
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                return Equals(obj as TestObjectBase);
            }

            public override int GetHashCode() { return Name != null ? Name.GetHashCode() : 0; }
        }

        private class TestObject : TestObjectBase
        {
            public TestObject() { }

            public TestObject(string name) : base(name) { }
        }

        private class TestObjectWithDataConstructor : TestObjectBase
        {
            public TestObjectWithDataConstructor(string name) : base(name) { }

            public TestObjectWithDataConstructor(ObjectData data) { Name = data.GetUnderlyingValue<string>("Name"); }
        }

        [Test]
        public void ObjectConvert() { Convert(new TestObject("test"), ObjectDataConverter.Default); }

        [Test]
        public void ObjectConvertByDataConstructor()
        {
            Convert(new TestObjectWithDataConstructor("test"), ObjectDataConverter.Default);
        }

        private static void Convert(object @object, ObjectDataConverter converter)
        {
            var data = converter.ToData(@object);
            Assert.AreEqual(@object.GetType(), data.ObjectType);

            var obj = converter.ToObject(data);
            Assert.AreEqual(@object, obj);
        }
    }
}