using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;

namespace ShuHai.Unity.Editor
{
    [InitializeOnLoad]
    public abstract class RootComponentEditor
    {
        public readonly IRootComponent Target;

        public RootComponentEditor(IRootComponent target) { Target = target; }

        public virtual void GUI() { }

        #region Create

        public static RootComponentEditor Create(IRootComponent component)
        {
            Ensure.Argument.NotNull(component, nameof(component));

            return _componentToEditorType.TryGetValue(component.GetType(), out var editorType)
                ? (RootComponentEditor)Activator.CreateInstance(editorType, component) : null;
        }

        #endregion Create

        #region Type Mapping

        private static readonly IReadOnlyDictionary<Type, Type> _componentToEditorType;

        private static IReadOnlyDictionary<Type, Type> CreateComponentToEditorTypeMapping()
        {
            return TypeCache.Get<RootComponentEditor>().DerivedTypes
                .Where(t => !t.IsAbstract)
                .Select(t => (type: t, attribute: t.GetCustomAttribute<TargetTypeAttribute>()))
                .Where(t => t.attribute != null)
                .ToDictionary(t => t.attribute.Type, t => t.type);
        }

        #endregion Type Mapping

        static RootComponentEditor() { _componentToEditorType = CreateComponentToEditorTypeMapping(); }
    }
}