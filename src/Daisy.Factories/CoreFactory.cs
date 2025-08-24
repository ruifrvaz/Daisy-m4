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
            foreach (var coreAssemblyName in settings.Workflows)
            {
                var assembly = Assembly.LoadFrom($"{coreAssemblyName}.dll");
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