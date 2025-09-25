using Daisy.Resources.Interfaces;
using Daisy.Resources.Models;
using System;
using System.Linq;

namespace Daisy.Factories
{
    public sealed class WorkflowFactory
    {
        // find all cores
        public static void LoadCores(ApplicationSettings settings, IServiceProvider serviceProvider)
        {
            var coreInterface = typeof(ICore);

            // Get all loaded assemblies that match the configured workflow names
            var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies()
                .Where(assembly => settings.Workflows.Any(workflowName => 
                    assembly.GetName().Name.Equals(workflowName, StringComparison.OrdinalIgnoreCase)))
                .ToList();

            foreach (var assembly in loadedAssemblies)
            {
                var coreTypes = assembly.GetTypes()
                    .Where(type => coreInterface.IsAssignableFrom(type) && type.IsClass)
                    .ToList();

                foreach (var coreType in coreTypes)
                {
                    var core = (ICore)Activator.CreateInstance(coreType);
                    Resources.Pools.Cores.Instance.Pool.Add(core);
                }
            }
        }
    }
}