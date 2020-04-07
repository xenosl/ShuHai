using System.Collections.Generic;
using NUnit.Framework;
using ShuHai.XConverts;

namespace ShuHai.EditSystem.Tests
{
    public class EditorConvertersTests
    {
        [Test]
        public void Convert()
        {
            var settings = new XConvertSettings
            {
                Converters = new XConverterCollection(new XConverter[]
                {
                    new EditorConverter(), new EditorObjectConverter()
                })
            };

            var editor = new Editor();

            var objects = new List<EditorObject>
            {
                editor.AddObject(12),
                editor.AddObject(12.033f)
            };

            var element = editor.ToXElement("EditorConvert", settings);
            var editorFromElement = element.ToObject(settings);
        }
    }
}