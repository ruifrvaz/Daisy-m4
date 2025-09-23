using Daisy.Resources.Interfaces;
using Daisy.Resources.Models;
using Daisy.Resources.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Daisy.Factories
{
    public sealed class WorkflowFactory
    {
        // find all cores
        public static void LoadCores(ApplicationSettings settings, IServiceProvider serviceProvider)
        {
            // Load plugin assemblies into AppDomain first
            LoadPluginAssemblies(settings.Workflows);

            // Discover core types using the new PluginService
            var coreTypes = PluginService.DiscoverTypes<ICore>(settings.Workflows);

            foreach (var coreType in coreTypes)
            {
                var core = PluginService.CreateInstance<ICore>(coreType);
                if (core != null)
                {
                    Resources.Pools.Cores.Instance.Pool.Add(core);
                }
            }
        }

        private static void LoadPluginAssemblies(IEnumerable<string> configuredAssemblies)
        {
            var pluginsRoot = Path.Combine(AppContext.BaseDirectory, "plugins");
            if (!Directory.Exists(pluginsRoot))
                throw new Exception("Error loading modules: plugins folder not found.");

            foreach (var assemblyName in configuredAssemblies)
            {
                var pluginDir = Path.Combine(pluginsRoot, assemblyName);
                if (Directory.Exists(pluginDir))
                {
                    var mainDll = Path.Combine(pluginDir, $"{assemblyName}.dll");
                    if (File.Exists(mainDll))
                    {
                        try
                        {
                            Assembly.LoadFrom(mainDll);
                        }
                        catch
                        {
                            // Ignore load failures - some assemblies may already be loaded
                        }
                    }
                }
            }
        }
    }
}