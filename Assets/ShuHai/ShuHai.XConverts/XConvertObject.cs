using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace ShuHai.XConverts
{
    public sealed class XConvertObject
    {
        public readonly int ID;

        public readonly object Content;

        public string Name;
        
        public int Version { get; private set; } = 1;

        public XConvertObject(string name, object content) : this(NextID++, content, name) { }

        public XConvertObject(int id, object content, string name)
        {
            Ensure.Argument.NotNull(content, nameof(content));
            
            ID = id;
            Content = content;
            Name = name;

            instances.Add(ID, this);
        }

        #region Convert

        public XElement ToXElement()
        {
            var xcontent = new XElement("Content");

            return new XElement(Name,
                new XAttribute("ID", ID.ToString()),
                xcontent,
                new XElement("Version", Version.ToString()));
        }

        public void Populate(XElement xelem) { throw new NotImplementedException(); }

        public static XConvertObject Parse(XElement xelem)
        {
//            var xcontent = xelem.Element("Content");
//            
//            var obj = new XConvertObject(ParseID(xelem), xelem.Name.LocalName);
//            obj.Populate(xelem);
//            return obj;
            throw new NotImplementedException();
        }

        public static int ParseID(XElement xelem)
        {
            Ensure.Argument.NotNull(xelem, nameof(xelem));
            return int.Parse(xelem.Attribute("ID").Value);
        }

        #endregion Convert

        #region Management

        public static int NextID { get; private set; } = 1;

        public static bool Exists(int id) { return instances.ContainsKey(id); }

        public static XConvertObject Get(int id) { return instances.GetValue(id); }

        private static readonly Dictionary<int, XConvertObject> instances = new Dictionary<int, XConvertObject>();

        #endregion Management
    }
}