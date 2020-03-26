using System.Collections.Generic;
using NUnit.Framework;
using ShuHai.XConverts;

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

            var converter = new EditorConverter();
            var element = converter.ToXElement(editor, "EditorConvert");
            //var editorFromElement = XConvert.ToObject(element);
        }
    }
}