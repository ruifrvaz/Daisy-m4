using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Daisy.Resources.Services
{
    /// <summary>
    /// Simple plugin discovery service that uses standard .NET reflection
    /// to find and instantiate plugin types directly from loaded assemblies.
    /// This replaces the complex AssemblyLoadContext approach with a cleaner solution.
    /// </summary>
    public static class PluginService
    {
        /// <summary>
        /// Discovers and returns types that implement the specified interface from all loaded assemblies.
        /// This method searches through assemblies that are already loaded in the current AppDomain,
        /// avoiding the need for complex assembly loading contexts.
        /// </summary>
        /// <typeparam name="T">The interface type to search for</typeparam>
        /// <param name="includeAssemblyNames">Names of assemblies to include in the search. If null, searches all assemblies.</param>
        /// <returns>Collection of types that implement the specified interface</returns>
        public static IEnumerable<Type> DiscoverTypes<T>(IEnumerable<string>? includeAssemblyNames = null)
        {
            var interfaceType = typeof(T);
            var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();

            // Filter assemblies if specific names are provided
            if (includeAssemblyNames != null)
            {
                var includeNames = includeAssemblyNames.ToHashSet();
                loadedAssemblies = loadedAssemblies
                    .Where(a => includeNames.Contains(a.GetName().Name ?? string.Empty))
                    .ToArray();
            }

            foreach (var assembly in loadedAssemblies)
            {
                Type[] types;
                try
                {
                    types = assembly.GetTypes();
                }
                catch (ReflectionTypeLoadException ex)
                {
                    // Handle partial load failures by using only successfully loaded types
                    types = ex.Types.Where(t => t != null).ToArray()!;
                }

                foreach (var type in types)
                {
                    if (type is { IsClass: true, IsAbstract: false } &&
                        interfaceType.IsAssignableFrom(type))
                    {
                        yield return type;
                    }
                }
            }
        }

        /// <summary>
        /// Creates an instance of the specified type using the provided constructor arguments.
        /// This method handles dynamic instantiation with proper error handling.
        /// </summary>
        /// <typeparam name="T">The expected return type</typeparam>
        /// <param name="type">The type to instantiate</param>
        /// <param name="constructorArgs">Arguments to pass to the constructor</param>
        /// <returns>Instance of the specified type, or null if instantiation fails</returns>
        public static T? CreateInstance<T>(Type type, params object[] constructorArgs) where T : class
        {
            try
            {
                var instance = Activator.CreateInstance(type, constructorArgs);
                return instance as T;
            }
            catch (Exception)
            {
                // Log error in production - for now just return null
                return null;
            }
        }
    }
}