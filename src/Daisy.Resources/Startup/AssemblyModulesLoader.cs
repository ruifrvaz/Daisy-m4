using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;

namespace Daisy.Resources.Startup
{
    public class AssemblyModulesLoader : AssemblyLoadContext
    {
        private readonly AssemblyDependencyResolver _resolver;

        private static readonly List<AssemblyModulesLoader> _pluginContexts = new();

        public AssemblyModulesLoader(string pluginMainAssemblyPath)
            : base(isCollectible: true) => _resolver = new AssemblyDependencyResolver(pluginMainAssemblyPath);

        protected override Assembly? Load(AssemblyName assemblyName)
        {
            try
            {
                // Always prefer assemblies already known to the host to keep type identity consistent
                return AssemblyLoadContext.Default.LoadFromAssemblyName(assemblyName);
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
                if (!Directory.Exists(dir)) continue;

                // Prefer <Name>.dll; fall back to any dll that has a .deps.json next to it
                var mainDll = Path.Combine(dir, $"{name}.dll");
                if (!File.Exists(mainDll))
                {
                    mainDll = Directory.EnumerateFiles(dir, "*.dll", SearchOption.TopDirectoryOnly)
                                       .FirstOrDefault(f => File.Exists(Path.ChangeExtension(f, ".deps.json")));
                    if (mainDll is null) continue;
                }

                var alc = new AssemblyModulesLoader(mainDll);
                _pluginContexts.Add(alc);                   // keep context alive
                yield return alc.LoadFromAssemblyPath(mainDll);
            }
        }

        public static Type[] SafeGetExportedTypes(Assembly asm)
        {
            try { return asm.GetExportedTypes(); }
            catch (ReflectionTypeLoadException ex) { return ex.Types!.Where(t => t is not null)!.ToArray()!; }
        }
    }
}