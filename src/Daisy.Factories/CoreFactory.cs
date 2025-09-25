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
            // Load known workflow core types directly
            LoadCore<Daisy.Workflows.Starter.StarterCore>();
            LoadCore<Daisy.Workflows.Weather.WeatherCore>();
        }

        private static void LoadCore<T>() where T : class, new()
        {
            var coreInterface = typeof(ICore);
            var coreType = typeof(T);

            if (coreInterface.IsAssignableFrom(coreType) && coreType.IsClass)
            {
                var core = new T() as ICore;
                Resources.Pools.Cores.Instance.Pool.Add(core);
            }
        }
    }
}