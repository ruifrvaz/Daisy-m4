using Daisy.Resources.Interfaces;
using Daisy.Resources.Models;
using Daisy.Resources.Pools;
using System;
using System.Linq;
using System.Reflection;

namespace Daisy.Factories
{
    public sealed class TransmitterFactory
    {
        public static void LoadExternalTransmitters(ApplicationSettings settings, IServiceProvider serviceProvider)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => !a.IsDynamic && a.GetName().Name.StartsWith("Daisy.Transmitters."))
                .ToList();

            // look in the assembly and find all classes that implement IExternalTransmitter
            var transmitterInterface = typeof(IExternalTransmitter);
            foreach (var assembly in assemblies)
            {
                try
                {
                    var transmitterTypes = assembly.GetTypes()
                        .Where(type => transmitterInterface.IsAssignableFrom(type) && type.IsClass && !type.IsAbstract)
                        .ToList();

                    foreach (var transmitterType in transmitterTypes)
                    {
                        // transmitters may or may not implement external IExternalTransmitter
                        if (transmitterType != null)
                        {
                            dynamic transmitterObject = Activator.CreateInstance(transmitterType);
                            var transmitter = transmitterObject as IExternalTransmitter;
                            ExternalTransmitters.Instance.Pool.Add(transmitter);
                        }
                    }
                }
                catch (ReflectionTypeLoadException ex)
                {
                    Console.WriteLine($"Warning: Could not load all types from assembly {assembly.GetName().Name}: {ex.Message}");
                }
            }
        }

        public static void LoadLoopBackTransmitters(ApplicationSettings settings, IServiceProvider serviceProvider)
        {
            // look in the assembly and find all classes that implement ILoopBackTransmitter
            var transmitterInterface = typeof(ILoopBackTransmitter);

            var assemblies = AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => !a.IsDynamic && a.GetName().Name.StartsWith("Daisy.Transmitters."))
                .ToList();

            foreach (var assembly in assemblies)
            {
                try
                {
                    var receiverTypes = assembly.GetTypes()
                        .Where(type => transmitterInterface.IsAssignableFrom(type) && type.IsClass && !type.IsAbstract)
                        .ToList();

                    foreach (var transmitterType in receiverTypes)
                    {
                        // transmitters may or may not implement external ILoopBackTransmitter
                        if (transmitterType != null)
                        {
                            dynamic transmitterObject = Activator.CreateInstance(transmitterType);
                            var transmitter = transmitterObject as ILoopBackTransmitter;
                            LoopBackTransmitters.Instance.Pool.Add(transmitter);
                        }
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