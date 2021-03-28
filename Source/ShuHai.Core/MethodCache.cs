using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ShuHai
{
    public class MethodCache
    {
        #region Methods

        public static IReadOnlyCollection<MethodInfo> Methods { get; private set; }

        public static IReadOnlyCollection<MethodInfo> StaticMethods { get; private set; }

        public static IReadOnlyDictionary<Type, IReadOnlyCollection<MethodInfo>> MethodsByAttributeType
        {
            get;
            private set;
        }

        public static IReadOnlyDictionary<Type, IReadOnlyCollection<MethodInfo>> StaticMethodsByAttributeType
        {
            get;
            private set;
        }

        private static void RefreshMethods()
        {
            Methods = TypeCache.Types.SelectMany(t => t.GetMethods(BindingAttributes.DeclareAll)).ToArray();
            StaticMethods = TypeCache.Types.SelectMany(t => t.GetMethods(BindingAttributes.DeclareStatic)).ToArray();
            MethodsByAttributeType = Methods.ReadOnlyMembersByAttributeType();
            StaticMethodsByAttributeType = StaticMethods.ReadOnlyMembersByAttributeType();
        }

        #endregion Methods

        private static void OnAssemblyLoad(Assembly assembly) { RefreshMethods(); }

        static MethodCache()
        {
            RefreshMethods();
            AssemblyCache.AssemblyLoaded += OnAssemblyLoad;
        }
    }
}
