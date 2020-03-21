using System.Xml.Linq;

namespace ShuHai.XConverts.Converters
{
    /// <summary>
    ///     Abstract converter for all types that doesn't need any child.
    /// </summary>
    public abstract class ValueConverter : XConverter
    {
        #region Value To XElement

        protected virtual string ValueToString(object value, XConvertSettings settings) { return value.ToString(); }

        protected sealed override void PopulateXElementValue(
            XElement element, object @object, XConvertSettings settings)
        {
            element.Value = ValueToString(@object, settings);
        }

        protected sealed override void PopulateXElementChildren(
            XElement element, object @object, XConvertSettings settings)
        {
            // Nothing to do...
        }

        #endregion Value To XElement

        #region XElement To Value

        protected sealed override void PopulateObjectMembersImpl(
            object @object, XElement element, XConvertSettings settings)
        {
            // Nothing to do...
        }

        #endregion XElement To Value
    }
}