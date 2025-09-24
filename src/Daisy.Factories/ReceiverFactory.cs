using Daisy.Resources.Interfaces;
using Daisy.Resources.Models;
using System;
using System.Linq;
using System.Reflection;

namespace Daisy.Factories
{
    public sealed class ReceiverFactory
    {
        public static void LoadExternalReceivers(ApplicationSettings settings, IServiceProvider serviceProvider)
        {
            // find all assemblies that implement IExternalReceiver, create an instance for each of them
            // and load them into receiver pool
            var receiverInterface = typeof(IExternalReceiver);

            var assemblies = AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => !a.IsDynamic && a.GetName().Name.StartsWith("Daisy.Receivers."))
                .ToList();

            foreach (var receiverAssembly in assemblies)
            {
                try
                {
                    var assemblyName = receiverAssembly.GetName().Name!;
                    if (!settings.Receivers.ContainsKey(assemblyName)) continue;

                    var assemblySettings = settings.Receivers[assemblyName];
                    var receiverTypes = receiverAssembly.GetTypes()
                        .Where(type => receiverInterface.IsAssignableFrom(type) && type.IsClass && !type.IsAbstract)
                        .ToList();

                    foreach (var receiverType in receiverTypes)
                    {
                        dynamic receiverObject = Activator.CreateInstance(receiverType, [assemblySettings.RunOnCores]);
                        var receiver = receiverObject as IExternalReceiver;

                        Resources.Pools.ExternalReceivers.Instance.Pool.Add(receiver);
                    }
                }
                catch (ReflectionTypeLoadException ex)
                {
                    Console.WriteLine($"Warning: Could not load all types from assembly {receiverAssembly.GetName().Name}: {ex.Message}");
                }
            }
        }

        public static void LoadLoopBackReceivers(ApplicationSettings settings, IServiceProvider serviceProvider)
        {
            // find all assemblies that implement ILoopBackReceiver, create an instance for each of them
            // and load them into receiver pool
            var receiverInterface = typeof(ILoopBackReceiver);

            var assemblies = AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => !a.IsDynamic && a.GetName().Name.StartsWith("Daisy.Receivers."))
                .ToList();

            foreach (var assembly in assemblies)
            {
                try
                {
                    var receiverTypes = assembly.GetTypes()
                        .Where(type => receiverInterface.IsAssignableFrom(type) && type.IsClass && !type.IsAbstract)
                        .ToList();

                    foreach (var receiverType in receiverTypes)
                    {
                        // receivers may or may not implement ILoopBackReceiver receivers
                        dynamic receiverObject = Activator.CreateInstance(receiverType);
                        var receiver = receiverObject as ILoopBackReceiver;

                        Resources.Pools.LoopBackReceivers.Instance.Pool.Add(receiver);
                    }
                }
                catch (ReflectionTypeLoadException ex)
                {
                    Console.WriteLine($"Warning: Could not load all types from assembly {assembly.GetName().Name}: {ex.Message}");
                }
            }
        }

        public static void LoadEventReceivers(ApplicationSettings settings, IServiceProvider serviceProvider)
        {
            // find all assemblies that implement IEventReceiver, create an instance for each of them and load them into receiver pool
            var receiverInterface = typeof(IEventReceiver);

            var assemblies = AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => !a.IsDynamic && a.GetName().Name.StartsWith("Daisy.Receivers."))
                .ToList();

            foreach (var receiverAssembly in assemblies)
            {
                try
                {
                    var assemblyName = receiverAssembly.GetName().Name!;
                    if (!settings.Receivers.ContainsKey(assemblyName)) continue;

                    var assemblySettings = settings.Receivers[assemblyName];
                    var receiverTypes = receiverAssembly.GetTypes()
                        .Where(type => receiverInterface.IsAssignableFrom(type) && type.IsClass && !type.IsAbstract)
                        .ToList();

                    foreach (var receiverType in receiverTypes)
                    {
                        // receivers may or may not implement event receivers
                        dynamic receiverObject = Activator.CreateInstance(receiverType, [assemblySettings.RunOnCores]);
                        var receiver = receiverObject as IEventReceiver;

                        Resources.Pools.EventReceivers.Instance.Pool.Add(receiver);
                    }
                }
                catch (ReflectionTypeLoadException ex)
                {
                    Console.WriteLine($"Warning: Could not load all types from assembly {receiverAssembly.GetName().Name}: {ex.Message}");
                }
            }
        }
    }
}
