using Daisy.Resources.Interfaces;
using Daisy.Resources.Models;
using Daisy.Resources.Pools;
using System;
using System.Linq;

namespace Daisy.Factories
{
    public sealed class TransmitterFactory
    {
        public static void LoadExternalTransmitters(ApplicationSettings settings, IServiceProvider serviceProvider)
        {
            // look in assemblies and find all classes that implement IExternalTransmitter
            var transmitterInterface = typeof(IExternalTransmitter);

            // Get all loaded assemblies that match the configured transmitter names
            var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies()
                .Where(assembly => settings.Transmitters.Any(transmitterName => 
                    assembly.GetName().Name.Equals(transmitterName, StringComparison.OrdinalIgnoreCase)))
                .ToList();

            foreach (var assembly in loadedAssemblies)
            {
                var transmitterTypes = assembly.GetTypes()
                    .Where(type => transmitterInterface.IsAssignableFrom(type) && type.IsClass)
                    .ToList();

                foreach (var transmitterType in transmitterTypes)
                {
                    // transmitters may or may not implement external IExternalTransmitter
                    if (transmitterType != null)
                    {
                        var transmitter = (IExternalTransmitter)Activator.CreateInstance(transmitterType);
                        ExternalTransmitters.Instance.Pool.Add(transmitter);
                    }
                }
            }
        }

        public static void LoadLoopBackTransmitters(ApplicationSettings settings, IServiceProvider serviceProvider)
        {
            // look in assemblies and find all classes that implement ILoopBackTransmitter
            var transmitterInterface = typeof(ILoopBackTransmitter);

            // Get all loaded assemblies that match the configured transmitter names
            var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies()
                .Where(assembly => settings.Transmitters.Any(transmitterName => 
                    assembly.GetName().Name.Equals(transmitterName, StringComparison.OrdinalIgnoreCase)))
                .ToList();

            foreach (var assembly in loadedAssemblies)
            {
                var transmitterTypes = assembly.GetTypes()
                    .Where(type => transmitterInterface.IsAssignableFrom(type) && type.IsClass)
                    .ToList();

                foreach (var transmitterType in transmitterTypes)
                {
                    // transmitters may or may not implement external ILoopBackTransmitter
                    if (transmitterType != null)
                    {
                        var transmitter = (ILoopBackTransmitter)Activator.CreateInstance(transmitterType);
                        LoopBackTransmitters.Instance.Pool.Add(transmitter);
                    }
                }
            }
        }
    }
}