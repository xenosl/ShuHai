using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ShuHai
{
    public static class AssemblyCache
    {
        #region Specical Assemblies

        public static Assembly Mscorlib => typeof(object).Assembly;

        public static Assembly ShuHaiCore => typeof(AssemblyCache).Assembly;

        #endregion Specical Assemblies

        #region Assemblies

        /// <summary>
        ///     Occurs after any assembly loaded and <see cref="Assemblies" /> refreshed.
        /// </summary>
        public static event Action<Assembly> AssemblyLoaded;

        /// <summary>
        ///     Cached collection for all assembly instances in current application domain.
        /// </summary>
        public static IReadOnlyCollection<Assembly> Assemblies { get; private set; }

        public static IReadOnlyDictionary<string, IReadOnlyCollection<Assembly>> AssemblyByName { get; private set; }

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
            return AssemblyByName.TryGetValue(name, out var collection) ? collection.FirstOrDefault() : null;
        }

        private static void InitializeAssemblies()
        {
            RefreshAssemblies();
            AppDomain.CurrentDomain.AssemblyLoad += OnAssemblyLoad;
        }

        private static void RefreshAssemblies()
        {
            Assemblies = AppDomain.CurrentDomain.GetAssemblies();

            AssemblyByName = Assemblies.GroupBy(a => a.GetName().Name, a => a)
                .ToDictionary(g => g.Key, g => (IReadOnlyCollection<Assembly>)new HashSet<Assembly>(g));
        }

        private static void OnAssemblyLoad(object sender, AssemblyLoadEventArgs args)
        {
            // Renew the cached array instead of adding the newly loaded assembly instance
            // to avoid potential iteration on "Instances" property while adding element to it.
            RefreshAssemblies();

            AssemblyLoaded?.Invoke(args.LoadedAssembly);
        }

        #endregion Assemblies

        static AssemblyCache() { InitializeAssemblies(); }
    }
}
