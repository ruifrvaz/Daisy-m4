using Daisy.Resources.Interfaces;
using Daisy.Resources.Models;
using System;
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

            var assemblies = AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => !a.IsDynamic && a.GetName().Name.StartsWith("Daisy.Workflows."))
                .ToList();

            foreach (var assembly in assemblies)
            {
                try
                {
                    var coreTypes = assembly.GetTypes()
                        .Where(type => coreInterface.IsAssignableFrom(type) && type.IsClass && !type.IsAbstract)
                        .ToList();

                    foreach (var coreType in coreTypes)
                    {
                        dynamic coreObject = Activator.CreateInstance(coreType);
                        var core = coreObject as ICore;
                        Resources.Pools.Cores.Instance.Pool.Add(core);
                    }
                }
                catch (ReflectionTypeLoadException ex)
                {
                    Console.WriteLine($"Warning: Could not load all types from assembly {assembly.GetName().Name}: {ex.Message}");
                }
            }
        }
    }
}