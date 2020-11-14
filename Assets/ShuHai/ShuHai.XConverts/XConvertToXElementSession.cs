using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Linq;

namespace ShuHai.XConverts
{
    /// <summary>
    ///     Stores convert information during a single convert session.
    /// </summary>
    public class XConvertToXElementSession
    {
        #region Object To ID

        public long GetOrGenerateID(object @object) { return GetOrGenerateID(@object, out _); }

        public long GetOrGenerateID(object @object, out bool firstTime)
        {
            Ensure.Argument.NotNull(@object, nameof(@object));
            if (@object.GetType().IsValueType)
                throw new ArgumentException("Reference type expected.", nameof(@object));

            return _idGenerator.GetId(@object, out firstTime);
        }

        public bool TryGetID(object @object, out long id)
        {
            Ensure.Argument.NotNull(@object, nameof(@object));

            id = _idGenerator.HasId(@object, out var firstTime);
            return !firstTime;
        }

        public bool HasID(object @object)
        {
            Ensure.Argument.NotNull(@object, nameof(@object));

            _idGenerator.HasId(@object, out var firstTime);
            return !firstTime;
        }

        private readonly ObjectIDGenerator _idGenerator = new ObjectIDGenerator();

        #endregion Object To ID

        #region Object To XElement

        public bool AddElement(object @object, XElement element)
        {
            if (_objectToElement.TryGetValue(@object, out var currentElement))
            {
                if (element != currentElement)
                    throw new InvalidOperationException("Attempt to bind one object to different element.");
                return false;
            }
            _objectToElement.Add(@object, element);
            return true;
        }

        public XElement GetElement(object @object)
        {
            Ensure.Argument.NotNull(@object, nameof(@object));

            return _objectToElement.TryGetValue(@object, out var element) ? element : null;
        }

        /// <summary>
        ///     Mapping from object to its converted XElement.
        /// </summary>
        private readonly Dictionary<object, XElement> _objectToElement = new Dictionary<object, XElement>();

        #endregion Object To XElement
    }
}