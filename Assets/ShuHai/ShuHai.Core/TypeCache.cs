using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ShuHai
{
    using ITypeCollection = IReadOnlyCollection<Type>;
    using IMethodDict = IReadOnlyDictionary<string, IReadOnlyList<MethodInfo>>;

    public sealed class TypeCache
    {
        public Type Type { get; }

        private TypeCache(Type type)
        {
            Type = type;

            _instances.Add(type, this);
        }

        private void Clear()
        {
            ClearCachedTypes();
            ClearCachedMembers();
        }

        #region Cached Types

        public ITypeCollection AssignableTypes =>
            _assignableTypes ?? (_assignableTypes = Type.EnumerateSelfAndDerivedTypes(SearchAssemblies).ToArray());

        /// <summary>
        ///     All derived types of current instance.
        /// </summary>
        public ITypeCollection DerivedTypes =>
            _derivedTypes ?? (_derivedTypes = Type.EnumerateDerivedTypes(SearchAssemblies).ToArray());

        /// <summary>
        ///     Derived types directly inherit from current instance.
        /// </summary>
        public ITypeCollection DirectDerivedTypes =>
            _directDerivedTypes ?? (_directDerivedTypes = Type.EnumerateDirectDerivedTypes(SearchAssemblies).ToArray());

        /// <summary>
        ///     All the interfaces implemented or inherited by the current <see cref="Type" />.
        /// </summary>
        public ITypeCollection Interfaces => _interfaces ?? (_interfaces = Type.GetInterfaces());

        /// <summary>
        ///     Most derived interfaces implemented or inherited by the current <see cref="Type" />.
        /// </summary>
        public ITypeCollection MostDerivedParentInterfaces =>
            _mostDerivedParentInterfaces ?? (_mostDerivedParentInterfaces = Type.GetMostDerivedInterfaces());

        /// <summary>
        ///     Least derived child interfaces of the current interface <see cref="Type" />.
        /// </summary>
        public ITypeCollection LeastDerivedChildInterfaces
        {
            get
            {
                if (_leastDerivedChildInterfaces == null)
                {
                    _leastDerivedChildInterfaces = SearchAssemblies.SelectMany(a => a.GetTypes())
                        .Where(t => t.IsInterface && t.GetMostDerivedInterfaces().Contains(Type))
                        .ToList();
                }
                return _leastDerivedChildInterfaces;
            }
        }

        /// <summary>
        ///     All constructed types of this type. Only works if this type is a generic type definition.
        /// </summary>
        public ITypeCollection ConstructedTypes =>
            _constructedTypes ?? (_constructedTypes = Type.EnumerateConstructedTypes(SearchAssemblies).ToArray());

        private ITypeCollection _assignableTypes;
        private ITypeCollection _derivedTypes;
        private ITypeCollection _directDerivedTypes;
        private ITypeCollection _interfaces;
        private ITypeCollection _mostDerivedParentInterfaces;
        private ITypeCollection _leastDerivedChildInterfaces;
        private ITypeCollection _constructedTypes;

        private void ClearCachedTypes()
        {
            _assignableTypes = null;
            _derivedTypes = null;
            _directDerivedTypes = null;
            _interfaces = null;
            _mostDerivedParentInterfaces = null;
            _leastDerivedChildInterfaces = null;
            _constructedTypes = null;
        }

        #endregion Cached Types

        #region Cached Members

        public IMethodDict Methods => _methods ?? (_methods = CollectMethods());

        private IMethodDict _methods;

        private IMethodDict CollectMethods()
        {
            return Type.GetMethods(BindingAttrForAll)
                .GroupBy(m => m.Name)
                .ToDictionary(g => g.Key, g => (IReadOnlyList<MethodInfo>)g.ToArray());
        }

        private void ClearCachedMembers() { _methods = null; }

        private const BindingFlags BindingAttrForAll = BindingFlags.DeclaredOnly |
                                                       BindingFlags.Public | BindingFlags.NonPublic |
                                                       BindingFlags.Static | BindingFlags.Instance;

        #endregion Cached Members

        #region Instances

        public static implicit operator TypeCache(Type type) { return Get(type); }
        public static implicit operator Type(TypeCache cache) { return cache.Type; }

        public static TypeCache Get<T>() { return Get(typeof(T)); }

        public static TypeCache Get(Type type)
        {
            if (type == null)
                return null;

            if (!_instances.TryGetValue(type, out var inst))
                inst = new TypeCache(type);
            return inst;
        }

        private static readonly Dictionary<Type, TypeCache> _instances = new Dictionary<Type, TypeCache>();

        #endregion Instances

        #region Types By Name

        /// <summary>
        ///     Get certain type of specified name if it is already cached; otherwise search the type from
        ///     <see cref="SearchAssemblies" />, get the the if exists or construct it if it is a generic type or an array.
        /// </summary>
        /// <param name="name">Full name of the type to get.</param>
        /// <param name="throwOnError"></param>
        public static Type GetType(string name, bool throwOnError)
        {
            var type = GetType(name);
            if (type == null && throwOnError)
                throw new TypeLoadException($@"Type ""{name}"" load failed.");
            return type;
        }

        public static Type GetType(string name)
        {
            Ensure.Argument.NotNullOrEmpty(name, nameof(name));
            return GetType(TypeName.Get(name));
        }

        public static Type GetType(TypeName name)
        {
            Ensure.Argument.NotNull(name, nameof(name));

            if (!_typesByName.TryGetValue(name, out var type))
            {
                type = MakeType(name);
                if (type != null)
                    _typesByName.Add(name, type);
            }
            return type;
        }

        private static readonly Dictionary<TypeName, Type> _typesByName = new Dictionary<TypeName, Type>();

        private static Type MakeType(TypeName name)
        {
            Type type = null;

            string typeName = name.DeclareName, asmName = name.AssemblyName;
            if (!name.IsGeneric)
            {
                if (string.IsNullOrEmpty(asmName))
                {
                    type = FindType(typeName);
                }
                else
                {
                    var asm = Assembly.Load(asmName);
                    type = asm != null ? asm.GetType(typeName) : null;
                }
            }
            else
            {
                var typeDef = FindType(typeName);
                if (typeDef != null)
                {
                    var argTypes = name.GenericArguments.Select(MakeType).ToArray();
                    type = argTypes.All(t => t != null) ? typeDef.MakeGenericType(argTypes) : null;
                }
            }

            if (type != null)
            {
                if (name.IsArray)
                {
                    type = name.ArrayRanks.Aggregate(type,
                        (current, rank) => rank == 1 ? current.MakeArrayType() : current.MakeArrayType(rank));
                }
            }

            return type;
        }

        private static Type FindType(string name)
        {
            foreach (var assembly in SearchAssemblies)
            {
                if (assembly == null)
                    continue;
                var type = assembly.GetType(name);
                if (type != null)
                    return type;
            }
            return null;
        }

        #endregion Types By Name

        #region Search Assemblies

        private static IReadOnlyCollection<Assembly> SearchAssemblies
        {
            get
            {
                if (!_assemblyEventRegistered)
                {
                    Assemblies.Loaded += OnAssemblyLoad;
                    _assemblyEventRegistered = true;
                }
                return Assemblies.Instances;
            }
        }

        private static bool _assemblyEventRegistered;

        private static void OnAssemblyLoad(Assembly assembly)
        {
            foreach (var instance in _instances.Values)
                instance.Clear();
        }

        #endregion Search Assemblies
    }
}
