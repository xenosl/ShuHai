using System;
using System.Runtime.Serialization.Formatters;
using System.Xml.Linq;

namespace ShuHai.XConverts.Converters
{
    [XConvertType(typeof(string))]
    public sealed class StringConverter : ValueConverter
    {
        public new static StringConverter Default { get; } = new StringConverter();

        #region String To XElement

        public XElement ToXElement(string str,
            string elementName, XConvertSettings settings = null, XConvertToXElementSession session = null)
        {
            Ensure.Argument.NotNullOrEmpty(elementName, nameof(elementName));

            settings = settings ?? XConvertSettings.Default;
            if (str == null)
                return CreateNullXElement(elementName, settings.AssemblyNameStyle);

            var element = new XElement(elementName);
            PopulateXElement(element, str, settings, session ?? new XConvertToXElementSession());
            return element;
        }

        protected override void PopulateXElementAttributes(XElement element,
            object @object, XConvertSettings settings, XConvertToXElementSession session)
        {
            XConvert.WriteObjectType(element, ConvertType, settings.AssemblyNameStyle);
        }

        #endregion String To XElement

        #region XElement To String

        public string ToString(XElement element, XConvertSettings settings = null)
        {
            Ensure.Argument.NotNull(element, nameof(element));

            return (string)CreateObject(element, ConvertType, settings);
        }

        protected override object CreateObject(XElement element, Type type, XConvertSettings settings)
        {
            return type == null ? null : element.Value;
        }

        #endregion XElement To String
    }
}