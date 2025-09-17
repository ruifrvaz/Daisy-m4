using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;

namespace Daisy.Resources.Startup
{
    public class AssemblyPluginsLoader : AssemblyLoadContext
    {
        private readonly AssemblyDependencyResolver _resolver;

        private static readonly List<AssemblyPluginsLoader> _pluginContexts = new();

        public AssemblyPluginsLoader(string pluginMainAssemblyPath)
            : base(isCollectible: true) => _resolver = new AssemblyDependencyResolver(pluginMainAssemblyPath);

        protected override Assembly? Load(AssemblyName assemblyName)
        {
            try
            {
                // Always prefer assemblies already known to the host to keep type identity consistent
                return Default.LoadFromAssemblyName(assemblyName);
            }
            catch (FileNotFoundException)
            {
                var path = _resolver.ResolveAssemblyToPath(assemblyName);
                return path is null ? null : LoadFromAssemblyPath(path);
            }
        }

        protected override IntPtr LoadUnmanagedDll(string name)
        {
            var path = _resolver.ResolveUnmanagedDllToPath(name);
            return path is null ? IntPtr.Zero : LoadUnmanagedDllFromPath(path);
        }

        public static IEnumerable<Assembly> LoadFromPluginsFolder(string pluginsRoot, IEnumerable<string> names)
        {
            foreach (var name in names)
            {
                var dir = Path.Combine(pluginsRoot, name);
                if (!Directory.Exists(dir))
                    continue;

                // Prefer <Name>.dll; fall back to any dll that has a .deps.json next to it
                var mainDll = Path.Combine(dir, $"{name}.dll");
                if (!File.Exists(mainDll))
                {
                    mainDll = Directory.EnumerateFiles(dir, "*.dll", SearchOption.TopDirectoryOnly)
                                       .FirstOrDefault(f => File.Exists(Path.ChangeExtension(f, ".deps.json")));

                    if (mainDll is null)
                        continue;
                }

                // Check if assembly is already loaded in the default context
                var assemblyName = AssemblyName.GetAssemblyName(mainDll);
                var existingAssembly = AppDomain.CurrentDomain.GetAssemblies()
                    .FirstOrDefault(a => AssemblyName.ReferenceMatchesDefinition(assemblyName, a.GetName()));

                if (existingAssembly != null)
                {
                    // Use the existing assembly from the default context to maintain type identity
                    yield return existingAssembly;
                }
                else
                {
                    // Load in plugin context only if not already available in default context
                    var alc = new AssemblyPluginsLoader(mainDll);
                    _pluginContexts.Add(alc);                   // keep context alive
                    yield return alc.LoadFromAssemblyPath(mainDll);
                }
            }
        }

        public static Type[] SafeGetExportedTypes(Assembly asm)
        {
            try { return asm.GetExportedTypes(); }
            catch (ReflectionTypeLoadException ex) { return ex.Types!.Where(t => t is not null)!.ToArray()!; }
        }
    }
}