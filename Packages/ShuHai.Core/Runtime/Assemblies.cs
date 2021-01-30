using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ShuHai
{
    public static class Assemblies
    {
        #region Specical Instances

        public static Assembly Mscorlib => typeof(object).Assembly;

        public static Assembly ShuHaiCore => typeof(Assemblies).Assembly;

        #endregion Specical Instances

        #region Cached Instances

        /// <summary>
        ///     Occurs after any assembly loaded and <see cref="Instances" /> refreshed.
        /// </summary>
        public static event Action<Assembly> Loaded;

        /// <summary>
        ///     Cached collection for all assembly instances in current application domain.
        /// </summary>
        public static IReadOnlyCollection<Assembly> Instances
        {
            get
            {
                if (_instances == null)
                {
                    RefreshInstances();
                    AppDomain.CurrentDomain.AssemblyLoad += OnAssemblyLoad;
                }
                return _instances;
            }
        }

        public static Assembly Find(Func<Assembly, bool> match) { return _instances.FirstOrDefault(match); }

        /// <summary>
        ///     Get one of the assembly instances with specified name.
        /// </summary>
        /// <param name="name">Name of the assembly to get.</param>
        /// <returns>
        ///     <see cref="Assembly" /> object with the specified <paramref name="name" /> if found; otherwise,
        ///     <see langword="null" />.
        /// </returns>
        public static Assembly Get(string name)
        {
            return !_instancesByName.TryGetValue(name, out var set) ? null : set.FirstOrDefault();
        }

        public static IEnumerable<Assembly> GetAll(string name)
        {
            return !_instancesByName.TryGetValue(name, out var set) ? null : set;
        }

        private static Assembly[] _instances;

        // Convenient cache for searching.
        private static Dictionary<string, HashSet<Assembly>>
            _instancesByName = new Dictionary<string, HashSet<Assembly>>();

        private static void RefreshInstances()
        {
            _instances = AppDomain.CurrentDomain.GetAssemblies();

            _instancesByName = new Dictionary<string, HashSet<Assembly>>();
            foreach (var instance in _instances)
                _instancesByName.Add(instance.GetName().Name, instance);
        }

        private static void OnAssemblyLoad(object sender, AssemblyLoadEventArgs args)
        {
            var loaded = args.LoadedAssembly;

            // Renew the cached array instead of adding the newly loaded assembly instance
            // to avoid potential iteration on "Instances" property while adding element to it.
            RefreshInstances();

            Loaded?.Invoke(loaded);
        }

        #endregion Cached Instances
    }
}
