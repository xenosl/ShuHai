using System.Xml.Linq;
using UnityEngine;

namespace ShuHai.XConverts.Unity
{
    [XConvertType(typeof(Behaviour))]
    public abstract class BehaviourConverter : ComponentConverter
    {
        private static class MemberNames
        {
            public const string Name = "enabled";
        }

        protected override void PopulateXElementChildren(XElement element, object @object, XConvertSettings settings)
        {
            base.PopulateXElementChildren(element, @object, settings);
            
            var b = (Behaviour)@object;
        }
    }
}