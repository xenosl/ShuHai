using System.Collections.Generic;
using System.Linq;
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

        #region Utilities

        protected static string MergeValues(IEnumerable<float> values, ValueStyle style)
        {
            return string.Join(",", values.Select(v => PrimitiveConverter.ToString(v, style)));
        }

        protected static float[] SplitValues(string value, ValueStyle style)
        {
            return value.Split(',').Select(v => PrimitiveConverter.ToSingle(v, style)).ToArray();
        }

        #endregion Utilities
    }
}