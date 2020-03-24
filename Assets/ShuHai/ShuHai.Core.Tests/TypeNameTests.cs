using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters;
using NUnit.Framework;

namespace ShuHai
{
    public class TypeNameTests
    {
        #region Test Generic Type

        public static readonly Type GenericType =
            typeof(Dictionary<string, KeyValuePair<TypeCache, Type>>);

        [Test]
        public void GenericTypeTests()
        {
            // System.Collections.Generic.Dictionary`2
            // [
            //   System.String,
            //   System.Collections.Generic.KeyValuePair`2
            //   [
            //     ShuHai.TypeName, UnityEditor.SceneManagement.EditorSceneManager
            //   ]
            // ]
            GenericTypeTests(TypeName.Get(GenericType.ToString()), false);

            // System.Collections.Generic.Dictionary`2
            // [
            //   [System.String, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089],
            //   [System.Collections.Generic.KeyValuePair`2
            //   [
            //     [ShuHai.TypeName, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null],
            //     [UnityEditor.SceneManagement.EditorSceneManager, UnityEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]
            //   ], mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]
            // ], mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
            GenericTypeTests(TypeName.Get(GenericType.AssemblyQualifiedName), true);
        }

        private void GenericTypeTests(TypeName root, bool hasAssembly)
        {
            var rootType = GenericType;
            InstanceTest(rootType, root, hasAssembly);
            Assert.AreEqual(Type.GetType(root.DeclareName), typeof(Dictionary<,>));
            Assert.Null(root.GenericParent);

            var arg0 = root.GetGenericArgument(0);
            InstanceTest<string>(arg0, hasAssembly);
            Assert.AreEqual(Type.GetType(arg0.DeclareName), typeof(string));
            Assert.AreEqual(arg0.GenericParent, root);

            var arg1 = root.GetGenericArgument(1);
            InstanceTest<KeyValuePair<TypeCache, Type>>(arg1, hasAssembly);
            Assert.AreEqual(Type.GetType(arg1.DeclareName), typeof(KeyValuePair<,>));
            Assert.AreEqual(arg1.GenericParent, root);

//            var arg10 = arg1.GetGenericArgument(0);
//            TestInstance<TypeCache>(arg10, hasAssembly);
//            Assert.AreEqual(Assemblies.UserRuntime.GetType(arg10.DeclareName), typeof(TypeCache));
//            Assert.AreEqual(arg10.GenericParent, arg1);
//
//            var arg11 = arg1.GetGenericArgument(1);
//            TestInstance<EditorSceneManager>(arg11, hasAssembly);
//            Assert.AreEqual(Assemblies.UnityEditor.GetType(arg11.DeclareName), typeof(EditorSceneManager));
//            Assert.AreEqual(arg11.GenericParent, arg1);
        }

        #endregion

        #region Test Nested Generic Type

        public static readonly Type NestedGenericArrayType =
            typeof(NestedGeneric<string, KeyValuePair<TypeCache, Type[,]>>[][]);

        [Test]
        public void NestedGenericArrayTypeTests()
        {
            // ShuHai.Tests.TypeNameFixture+NestedGeneric`2
            // [
            //   [System.String, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089],
            //   [System.Collections.Generic.KeyValuePair`2
            //   [
            //     [ShuHai.TypeCache, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null],
            //     [UnityEditor.SceneManagement.EditorSceneManager[,], UnityEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]
            //   ], mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]
            // ][][], Assembly-CSharp-Editor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
            NestedGenericArrayTypeTests(TypeName.Get(NestedGenericArrayType.AssemblyQualifiedName), true);

            // ShuHai.Tests.TypeNameFixture+NestedGeneric`2
            // [
            //   System.String,System.Collections.Generic.KeyValuePair`2
            //   [
            //     ShuHai.TypeCache,
            //     UnityEditor.SceneManagement.EditorSceneManager[,]
            //   ]
            // ][][]
            NestedGenericArrayTypeTests(TypeName.Get(NestedGenericArrayType.ToString()), false);
        }

        private void NestedGenericArrayTypeTests(TypeName root, bool hasAssembly)
        {
            var rootType = NestedGenericArrayType;
            InstanceTest(rootType, root, hasAssembly);
            Assert.Null(root.GenericParent);

            var arg0 = root.GetGenericArgument(0);
            InstanceTest<string>(arg0, hasAssembly);
            Assert.AreEqual(root, arg0.GenericParent);

            var arg1 = root.GetGenericArgument(1);
            InstanceTest<KeyValuePair<TypeCache, Type[,]>>(arg1, hasAssembly);
            Assert.AreEqual(root, arg1.GenericParent);

            var arg10 = arg1.GetGenericArgument(0);
            InstanceTest<TypeCache>(arg10, hasAssembly);
            Assert.AreEqual(arg1, arg10.GenericParent);

            var arg11 = arg1.GetGenericArgument(1);
            InstanceTest<Type[,]>(arg11, hasAssembly);
            Assert.AreEqual(arg1, arg11.GenericParent);
        }

        private class NestedGeneric<TKey, TValue> : IDictionary<TKey, TValue>
        {
            public ICollection<TKey> Keys
            {
                get { return dict.Keys; }
            }

            public ICollection<TValue> Values
            {
                get { return dict.Values; }
            }

            public NestedGeneric(Dictionary<TKey, TValue> dict) { this.dict = dict; }

            public void Clear() { dict.Clear(); }

            public int Count
            {
                get { return dict.Count; }
            }

            public void Add(TKey key, TValue value) { dict.Add(key, value); }
            public bool ContainsKey(TKey key) { return dict.ContainsKey(key); }
            public bool Remove(TKey key) { return dict.Remove(key); }
            public bool TryGetValue(TKey key, out TValue value) { return dict.TryGetValue(key, out value); }

            public TValue this[TKey key]
            {
                get { return dict[key]; }
                set { dict[key] = value; }
            }

            public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() { return dict.GetEnumerator(); }
            IEnumerator IEnumerable.GetEnumerator() { return ((IEnumerable)dict).GetEnumerator(); }

            bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly
            {
                get { return collection.IsReadOnly; }
            }

            void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item) { collection.Add(item); }

            bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
            {
                return collection.Contains(item);
            }

            void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
            {
                collection.CopyTo(array, arrayIndex);
            }

            bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
            {
                return collection.Remove(item);
            }

            private ICollection<KeyValuePair<TKey, TValue>> collection
            {
                get { return dict; }
            }

            private readonly Dictionary<TKey, TValue> dict;
        }

        #endregion

        private static void InstanceTest<T>(TypeName name, bool hasAssembly) { InstanceTest(typeof(T), name, hasAssembly); }

        private static void InstanceTest(Type type, TypeName name, bool hasAssembly)
        {
            var isArray = type.IsArray;
            Type elementType = null;
            int arrayDeclareCount = 0;
            if (isArray)
            {
                elementType = type;
                while (elementType.IsArray)
                {
                    Assert.AreEqual(elementType.GetArrayRank(), name.GetArrayRank(arrayDeclareCount));
                    elementType = elementType.GetElementType();
                    arrayDeclareCount++;
                }
            }
            var elementOrType = elementType ?? type;
            Assert.AreEqual(arrayDeclareCount, name.ArrayDeclareCount);

            var isGeneric = elementOrType.IsGenericType;
            var defType = isGeneric ? elementOrType.GetGenericTypeDefinition() : null;
            var defOrType = defType ?? elementOrType;

            Assert.AreEqual(hasAssembly ? type.FullName : type.ToString(), name.FullName);
            Assert.AreEqual(defOrType.FullName, name.DeclareName);
            Assert.AreEqual(elementOrType.Namespace, name.Namespace);
            Assert.AreEqual(elementOrType.Name, name.Name);
            Assert.AreEqual(hasAssembly ? type.Assembly.FullName : null, name.AssemblyName);

            Assert.AreEqual(isGeneric, name.IsGeneric);
            if (isGeneric)
            {
                var args = elementOrType.GetGenericArguments();
                Assert.AreEqual(args.Length, name.GenericArgumentCount);
            }

            var expectedTypeStr = hasAssembly ? type.AssemblyQualifiedName : type.ToString();
            var assemblyNameStyle = hasAssembly ? FormatterAssemblyStyle.Full : FormatterAssemblyStyle.Simple;
            Assert.AreEqual(expectedTypeStr, name.ToString(true, assemblyNameStyle));
        }
    }
}