using Daisy.Resources.Interfaces;
using Daisy.Resources.Models;
using Daisy.Resources.Startup;
using System;
using System.IO;
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

            var pluginsRoot = Path.Combine(AppContext.BaseDirectory, "plugins");
            var assemblies = Directory.Exists(pluginsRoot)
                ? AssemblyModulesLoader.LoadFromPluginsFolder(pluginsRoot, settings.Receivers.Keys).ToList() : throw new Exception("Error loading modules: plugins folder not found.");

            foreach (var receiverAssembly in assemblies)
            {
                var assemblySettings = settings.Receivers[receiverAssembly.GetName().Name!];
                var receiverTypes = receiverAssembly.GetTypes().Where(type => receiverInterface.IsAssignableFrom(type) && type.IsClass).ToList();
                foreach (var receiverType in receiverTypes)
                {
                    dynamic receiverObject;
                    if (assemblySettings.RunOnCores.Any())
                    {
                        receiverObject = Activator.CreateInstance(receiverType, [assemblySettings.RunOnCores]);
                    }
                    else
                    {
                        receiverObject = Activator.CreateInstance(receiverType);
                    }
                    var receiver = receiverObject as IExternalReceiver;

                    Resources.Pools.ExternalReceivers.Instance.Pool.Add(receiver);
                }
            }
        }

        public static void LoadLoopBackReceivers(ApplicationSettings settings, IServiceProvider serviceProvider)
        {
            // find all assemblies that implement ILoopBackReceiver, create an instance for each of them
            // and load them into receiver pool
            var receiverInterface = typeof(ILoopBackReceiver);
            
            var pluginsRoot = Path.Combine(AppContext.BaseDirectory, "plugins");
            var assemblies = Directory.Exists(pluginsRoot)
                ? AssemblyModulesLoader.LoadFromPluginsFolder(pluginsRoot, settings.Receivers.Keys).ToList() : throw new Exception("Error loading modules: plugins folder not found.");

            foreach (var assembly in assemblies)
            {
                var receiverTypes = assembly.GetTypes().Where(type => receiverInterface.IsAssignableFrom(type) && type.IsClass).ToList();
                foreach (var receiverType in receiverTypes)
                {
                    // receivers may or may not implement ILoopBackReceiver receivers

                    dynamic receiverObject = Activator.CreateInstance(receiverType);
                    var receiver = receiverObject as ILoopBackReceiver;

                    Resources.Pools.LoopBackReceivers.Instance.Pool.Add(receiver);
                }
            }
        }

        public static void LoadEventReceivers(ApplicationSettings settings, IServiceProvider serviceProvider)
        {
            // find all assemblies that implement IEventReceiver, create an instance for each of them and load them into receiver pool
            var receiverInterface = typeof(IEventReceiver);

            var pluginsRoot = Path.Combine(AppContext.BaseDirectory, "plugins");
            var assemblies = Directory.Exists(pluginsRoot)
                ? AssemblyModulesLoader.LoadFromPluginsFolder(pluginsRoot, settings.Receivers.Keys).ToList() : throw new Exception("Error loading modules: plugins folder not found.");

            foreach (var receiverAssembly in assemblies)
            {
                var assemblySettings = settings.Receivers[receiverAssembly.GetName().Name!];
                var receiverTypes = receiverAssembly.GetTypes().Where(type => receiverInterface.IsAssignableFrom(type) && type.IsClass).ToList();
                foreach (var receiverType in receiverTypes)
                {
                    // receivers may or may not implement event receivers
                    dynamic receiverObject;
                    if (assemblySettings.RunOnCores.Any())
                    {
                        receiverObject = Activator.CreateInstance(receiverType, [assemblySettings.RunOnCores]);
                    }
                    else
                    {
                        receiverObject = Activator.CreateInstance(receiverType);
                    }
                    var receiver = receiverObject as IEventReceiver;

                    Resources.Pools.EventReceivers.Instance.Pool.Add(receiver);
                }
            }
        }
    }
}
