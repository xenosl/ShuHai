using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace ShuHai.Unity
{
    /// <summary>
    ///     Provides entry points of Unity events in fixed invocation order.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         The Unity events (such as Awake, Start, Update, etc) is invoked in an undetermined order by default, users
    ///         have to take care of in which order the custom scripts are executed by configure project settings to make
    ///         sure the programme behaves the same each time in different software or hardware environment. This class
    ///         invokes the event methods of its' components in a fixed order by default, thus the programme behaves the
    ///         same in any environment.
    ///     </para>
    ///     <para>
    ///         To take advantage of the class, implement the <see cref="IRootComponent" /> interface, the class is going
    ///         to create an instance of the implemented type using the default constructor (in such case a default constructor
    ///         is required).
    ///     </para>
    ///     <para>
    ///         To change invocation order of specific type of component, apply <see cref="RootComponentAttribute" /> to
    ///         that type and set <see cref="RootComponentAttribute.Priority" /> field. The greater priority value makes
    ///         the instance of the type executes first, the same priority value makes the execution order undefined but
    ///         determined every time in all situations.
    ///     </para>
    /// </remarks>
    public static class Root
    {
        #region Life Cycle

        public static event Action FixedUpdating;
        public static event Action FixedUpdated;
        public static event Action Updating;
        public static event Action Updated;
        public static event Action LateUpdating;
        public static event Action LateUpdated;

        internal static void Initialize() { InitializeComponents(); }

        internal static void Deinitialize() { DeinitializeComponents(); }

        internal static void FixedUpdate()
        {
            FixedUpdating?.Invoke();
            ComponentFixedUpdate();
            FixedUpdated?.Invoke();
        }

        internal static void Update()
        {
            Updating?.Invoke();
            ComponentsUpdate();
            Updated?.Invoke();
        }

        internal static void LateUpdate()
        {
            LateUpdating?.Invoke();
            ComponentLateUpdate();
            LateUpdated?.Invoke();
        }

#if UNITY_EDITOR

        public static event Action EditorUpdate;

        internal static void EditorRootUpdate() { EditorUpdate?.Invoke(); }

#endif // UNITY_EDITOR

        #endregion Life Cycle

        #region Components

        public static IReadOnlyList<IRootComponent> Components => _components;

        public static T GetComponent<T>()
            where T : IRootComponent
        {
            return (T)_componentsByType.GetValue(typeof(T));
        }

        public static IRootComponent GetComponent(Type type)
        {
            Ensure.Argument.NotNull(type, nameof(type));
            Ensure.Argument.Is<IRootComponent>(type, nameof(type));
            return _componentsByType.GetValue(type);
        }

        private static readonly List<IRootComponent> _components = new List<IRootComponent>();

        private static readonly Dictionary<Type, IRootComponent>
            _componentsByType = new Dictionary<Type, IRootComponent>();

        #region Initialization

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        private static void CreateComponents()
        {
            var types = ShuHai.Assemblies.Instances
                .SelectMany(a => a.GetTypes())
                .Where(IsValidTypeForComponentCreation)
                .ToArray();
            types.ForEach(VerifyComponentType);

            _components.AddRange(types.Select(t => (IRootComponent)Activator.CreateInstance(t)));
            _components.Sort(CompareComponent);

            foreach (var component in _components)
                _componentsByType.Add(component.GetType(), component);
        }

        private static bool IsValidTypeForComponentCreation(Type type)
        {
            if (!typeof(IRootComponent).IsAssignableFrom(type))
                return false;
            if (type.IsAbstract)
                return false;
            if (type.IsGenericType && type.ContainsGenericParameters)
                return false;
            return true;
        }

        private static int CompareComponent(IRootComponent l, IRootComponent r)
        {
            Type lt = l.GetType(), rt = r.GetType();
            var la = lt.GetCustomAttribute<RootComponentAttribute>();
            var ra = rt.GetCustomAttribute<RootComponentAttribute>();

            int lp = la?.Priority ?? 0, rp = ra?.Priority ?? 0;
            int c = -lp.CompareTo(rp);
            if (c != 0)
                return c;

            // Make sure the invocation order keeps the same when priority is the same.
            return string.Compare(lt.FullName, rt.FullName, StringComparison.Ordinal);
        }

        private static void VerifyComponentType(Type type)
        {
            if (type.FindDefaultConstructor() == null)
            {
                throw new InvalidOperationException(
                    $"A default constructor({type}) is required as Root Component.");
            }
        }

        #endregion Initialization

        #region Events

        private static void InitializeComponents()
        {
            foreach (var component in _components)
                component.Initialize();
        }

        private static void DeinitializeComponents()
        {
            for (int i = _components.Count - 1; i >= 0; --i)
                _components[i].Deinitialize();
        }

        private static void ComponentFixedUpdate()
        {
            foreach (var c in _components)
                c.FixedUpdate();
        }

        private static void ComponentsUpdate()
        {
            foreach (var c in _components)
                c.Update();
        }

        private static void ComponentLateUpdate()
        {
            foreach (var c in _components)
                c.LateUpdate();
        }

        #endregion Events

        #endregion Components
    }
}
