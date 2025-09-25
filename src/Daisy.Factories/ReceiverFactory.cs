using Daisy.Resources.Interfaces;
using Daisy.Resources.Models;
using System;
using System.Linq;

namespace Daisy.Factories
{
    public sealed class ReceiverFactory
    {
        public static void LoadExternalReceivers(ApplicationSettings settings, IServiceProvider serviceProvider)
        {
            // find all assemblies that implement IExternalReceiver, create an instance for each of them
            // and load them into receiver pool
            var receiverInterface = typeof(IExternalReceiver);

            // Get all loaded assemblies that match the configured receiver names
            var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies()
                .Where(assembly => settings.Receivers.Keys.Any(receiverName =>
                    assembly.GetName().Name.Equals(receiverName, StringComparison.OrdinalIgnoreCase)))
                .ToList();

            foreach (var receiverAssembly in loadedAssemblies)
            {
                var assemblyName = receiverAssembly.GetName().Name!;
                if (!settings.Receivers.ContainsKey(assemblyName))
                    continue;

                var assemblySettings = settings.Receivers[assemblyName];
                var receiverTypes = receiverAssembly.GetTypes()
                    .Where(type => receiverInterface.IsAssignableFrom(type) && type.IsClass)
                    .ToList();

                foreach (var receiverType in receiverTypes)
                {
                    var receiver = (IExternalReceiver)Activator.CreateInstance(receiverType, assemblySettings.RunOnCores);
                    Resources.Pools.ExternalReceivers.Instance.Pool.Add(receiver);
                }
            }
        }

        public static void LoadLoopBackReceivers(ApplicationSettings settings, IServiceProvider serviceProvider)
        {
            // find all assemblies that implement ILoopBackReceiver, create an instance for each of them
            // and load them into receiver pool
            var receiverInterface = typeof(ILoopBackReceiver);

            // Get all loaded assemblies that match the configured receiver names
            var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies()
                .Where(assembly => settings.Receivers.Keys.Any(receiverName =>
                    assembly.GetName().Name.Equals(receiverName, StringComparison.OrdinalIgnoreCase)))
                .ToList();

            foreach (var assembly in loadedAssemblies)
            {
                var receiverTypes = assembly.GetTypes()
                    .Where(type => receiverInterface.IsAssignableFrom(type) && type.IsClass)
                    .ToList();

                foreach (var receiverType in receiverTypes)
                {
                    // receivers may or may not implement ILoopBackReceiver receivers
                    var receiver = (ILoopBackReceiver)Activator.CreateInstance(receiverType);
                    Resources.Pools.LoopBackReceivers.Instance.Pool.Add(receiver);
                }
            }
        }

        public static void LoadEventReceivers(ApplicationSettings settings, IServiceProvider serviceProvider)
        {
            // find all assemblies that implement IEventReceiver, create an instance for each of them and load them into receiver pool
            var receiverInterface = typeof(IEventReceiver);

            // Get all loaded assemblies that match the configured receiver names
            var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies()
                .Where(assembly => settings.Receivers.Keys.Any(receiverName =>
                    assembly.GetName().Name.Equals(receiverName, StringComparison.OrdinalIgnoreCase)))
                .ToList();

            foreach (var receiverAssembly in loadedAssemblies)
            {
                var assemblyName = receiverAssembly.GetName().Name!;
                if (!settings.Receivers.ContainsKey(assemblyName))
                    continue;

                var assemblySettings = settings.Receivers[assemblyName];
                var receiverTypes = receiverAssembly.GetTypes()
                    .Where(type => receiverInterface.IsAssignableFrom(type) && type.IsClass)
                    .ToList();

                foreach (var receiverType in receiverTypes)
                {
                    // receivers may or may not implement event receivers
                    var receiver = (IEventReceiver)Activator.CreateInstance(receiverType, assemblySettings.RunOnCores);
                    Resources.Pools.EventReceivers.Instance.Pool.Add(receiver);
                }
            }
        }
    }
}
