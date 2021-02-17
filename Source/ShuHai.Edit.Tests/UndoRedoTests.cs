using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace ShuHai.Edit.Tests
{
    public class UndoRedoTests
    {
        public class TestObject : IEquatable<TestObject>
        {
            public int ID { get; }

            public string Name { get; set; } = string.Empty;

            public bool Enabled { get; set; }

            public bool Equals(TestObject other)
            {
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;
                return ID == other.ID;
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                return Equals(obj as TestObject);
            }

            public override int GetHashCode() { return ID.GetHashCode(); }

            #region Instances

            private TestObject(int id) { ID = id; }

            public static TestObject Create() { return Create(_idGenerator++); }

            public static TestObject Create(ObjectData data)
            {
                var id = data.GetUnderlyingValue<int>("ID");
                if (_idGenerator <= id)
                    _idGenerator = id + 1;
                return Create(id);
            }

            private static TestObject Create(int id)
            {
                var obj = new TestObject(id);
                _instances.Add(obj.ID, obj);
                return obj;
            }

            public static bool Destroy(int id) { return _instances.Remove(id); }

            public static bool Exists(int id) { return _instances.ContainsKey(id); }

            public static TestObject Get(int id) { return _instances[id]; }

            private static int _idGenerator = 1;
            private static readonly Dictionary<int, TestObject> _instances = new Dictionary<int, TestObject>();

            #endregion Instances
        }

        [Test]
        public void UndoRedoChange()
        {
            var undoRedo = new UndoRedo();

            var obj = TestObject.Create();
            Assert.IsFalse(obj.Enabled);
            Assert.AreEqual(string.Empty, obj.Name);

//            undoRedo.Do(
//                args =>
//                {
//                    obj.Enabled = args.Enabled;
//                    obj.Name = args.Name;
//                }, (Enabled: true, Name: "Test"),
//                args =>
//                {
//                    obj.Enabled = args.Enabled;
//                    obj.Name = args.Name;
//                }, (Enabled: false, Name: string.Empty));
//            Assert.IsTrue(obj.Enabled);
//            Assert.AreEqual("Test", obj.Name);
//
//            undoRedo.Undo();
//            Assert.IsFalse(obj.Enabled);
//            Assert.AreEqual(string.Empty, obj.Name);
//
//            undoRedo.Redo();
//            Assert.IsTrue(obj.Enabled);
//            Assert.AreEqual("Test", obj.Name);
        }

        [Test]
        public void UndoRedoCreate()
        {
            var undoRedo = new UndoRedo();
            
   
            //var obj = TestObject.Create();
            //undoRedo.Do(null, null, new[] { obj });
        }
    }
}