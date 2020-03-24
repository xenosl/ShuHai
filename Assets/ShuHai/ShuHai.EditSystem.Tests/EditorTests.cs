using System.Collections.Generic;
using NUnit.Framework;

namespace ShuHai.EditSystem.Tests
{
    public class EditorTests
    {
        [Test]
        public void Serialization()
        {
            var editor = new Editor();
            
            var objects = new List<EditorObject>
            {
                editor.AddObject(12),
                editor.AddObject(12.033f)
            };

            CollectionAssert.AreEqual(objects, editor.Objects);
            var data = editor.Serialize("EditorTests");
            editor.Deserialize(data);
            //CollectionAssert.AreEqual(objects, editor.Objects);
        }
    }
}