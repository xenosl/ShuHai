using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using NUnit.Framework;

namespace ShuHai
{
    public class TypeExtensionsTests
    {
        [Test]
        public void GetMostDerivedInterfaces()
        {
            GetMostDerivedInterfaces(typeof(Dictionary<string, Type>),
                typeof(IDictionary<string, Type>), typeof(IReadOnlyDictionary<string, Type>),
                typeof(IDictionary), typeof(ISerializable), typeof(IDeserializationCallback));

            GetMostDerivedInterfaces(typeof(IDictionary<string, Type>),
                typeof(ICollection<KeyValuePair<string, Type>>));

            GetMostDerivedInterfaces(typeof(ICollection<string>), typeof(IEnumerable<string>));
        }

        private void GetMostDerivedInterfaces(Type type, params Type[] expect)
        {
            var actual = type.GetMostDerivedInterfaces();
            CollectionAssert.AreEquivalent(expect, actual);
        }

        #region Member Getters

        private abstract class TestClassBase
        {
            public readonly string Name;

            protected TestClassBase() : this(typeof(TestClassBase).Name) { }
            protected TestClassBase(string name) { Name = name; }

            public void PublicMethod() { }
            public void PublicMethod(string arg) { }
            protected void ProtectedMethod() { }
            private void PrivateMethod() { }

            static TestClassBase() { }
        }

        private class TestClassDerived1 : TestClassBase
        {
            public TestClassDerived1() : this(typeof(TestClassDerived1).Name) { }
            public TestClassDerived1(string name) : base(name) { }

            public void PublicMethod1() { }
        }

        private class TestClassDerived2 : TestClassBase
        {
            public void PublicMethod2() { }
        }

        [Test]
        public void FindConstructor()
        {
            var baseType = typeof(TestClassBase);
            Assert.IsNotNull(baseType.FindConstructor());
            Assert.IsNotNull(baseType.FindConstructor(typeof(string)));

            var derived1Type = typeof(TestClassDerived1);
            Assert.IsNotNull(derived1Type.FindConstructor());
            Assert.IsNotNull(derived1Type.FindConstructor(typeof(string)));
        }

        [Test]
        public void FindMethod()
        {
            var baseType = typeof(TestClassBase);
            Assert.AreEqual("PublicMethod", baseType.FindMethod("PublicMethod").Name);
            Assert.AreEqual("PublicMethod", baseType.FindMethod("PublicMethod", typeof(string)).Name);
            Assert.AreEqual("ProtectedMethod", baseType.FindMethod("ProtectedMethod").Name);
            Assert.AreEqual("PrivateMethod", baseType.FindMethod("PrivateMethod").Name);

            var derived1Type = typeof(TestClassDerived1);
            Assert.AreEqual("PublicMethod1", derived1Type.FindMethod("PublicMethod1").Name);
            Assert.IsNull(derived1Type.FindMethod("PublicMethod"));
            Assert.IsNull(derived1Type.FindMethod("ProtectedMethod"));
            Assert.IsNull(derived1Type.FindMethod("PrivateMethod"));
            Assert.IsNull(derived1Type.FindMethod("PublicMethod2"));
        }

        #endregion Member Getters
    }
}