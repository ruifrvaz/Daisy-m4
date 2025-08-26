using Daisy.Resources.Interfaces;
using Daisy.Resources.Models;
using Daisy.Resources.Startup;
using System;
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
            var coreInterface = typeof(ICore);

            var pluginsRoot = Path.Combine(AppContext.BaseDirectory, "plugins");
            var assemblies = Directory.Exists(pluginsRoot)
                ? AssemblyModulesLoader.LoadFromPluginsFolder(pluginsRoot, settings.Workflows).ToList()
                : throw new Exception("Error loading modules: plugins folder not found.");
            foreach (var assembly in assemblies)
            {
                var coreTypes = assembly.GetTypes()
                                            .Where(type => coreInterface.IsAssignableFrom(type) && type.IsClass)
                                            .ToList();

                foreach (var coreType in coreTypes)
                {
                    dynamic coreObject = Activator.CreateInstance(coreType);
                    var core = coreObject as ICore;
                    Resources.Pools.Cores.Instance.Pool.Add(core);
                }
            }
        }
    }
}