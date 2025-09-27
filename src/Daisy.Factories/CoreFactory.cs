using Daisy.Resources.Interfaces;
using Daisy.Resources.Models;
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

            // Load each configured workflow assembly individually
            foreach (var workflowName in settings.Workflows)
            {
                try
                {
                    Assembly assembly;
                    try
                    {
                        // Try to load using the assembly name first (this works for referenced assemblies)
                        assembly = Assembly.Load(workflowName);
                    }
                    catch (FileNotFoundException)
                    {
                        // Fallback: try loading from file path
                        var assemblyPath = Path.Combine(AppContext.BaseDirectory, $"{workflowName}.dll");
                        if (File.Exists(assemblyPath))
                        {
                            assembly = Assembly.LoadFrom(assemblyPath);
                        }
                        else
                        {
                            continue; // Skip if assembly cannot be found
                        }
                    }

                    var coreTypes = assembly.GetTypes()
                        .Where(type => coreInterface.IsAssignableFrom(type) && type.IsClass)
                        .ToList();

                    foreach (var coreType in coreTypes)
                    {
                        var core = (ICore)Activator.CreateInstance(coreType);
                        Resources.Pools.Cores.Instance.Pool.Add(core);
                    }
                }
                catch (Exception ex)
                {
                    // Log the error but continue loading other assemblies
                    Console.WriteLine($"Warning: Could not load workflow assembly {workflowName}: {ex.Message}");
                }
            }
        }
    }
}