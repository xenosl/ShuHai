using System;
using System.Xml.Linq;
using UnityEngine;

namespace ShuHai.XConverts.Unity
{
    [XConvertType(typeof(Component))]
    public abstract class ComponentConverter : ObjectConverter
    {
    }
}