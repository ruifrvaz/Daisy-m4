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

            foreach (var receiverAssemblyName in settings.Receivers)
            {
                var assembly = Assembly.LoadFrom($"{receiverAssemblyName}.dll");
                var receiverTypes = assembly.GetTypes().Where(type => receiverInterface.IsAssignableFrom(type) && type.IsClass).ToList();
                foreach (var receiverType in receiverTypes)
                {
                    // receivers may or may not implement external receivers

                    dynamic receiverObject = Activator.CreateInstance(receiverType);
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

            foreach (var receiverAssemblyName in settings.Receivers)
            {
                var assembly = Assembly.LoadFrom($"{receiverAssemblyName}.dll");
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

            foreach (var receiverAssemblyName in settings.Receivers)
            {
                var assembly = Assembly.LoadFrom($"{receiverAssemblyName}.dll");
                var receiverTypes = assembly.GetTypes().Where(type => receiverInterface.IsAssignableFrom(type) && type.IsClass).ToList();
                foreach (var receiverType in receiverTypes)
                {
                    // receivers may or may not implement event receivers
                    dynamic receiverObject = Activator.CreateInstance(receiverType);
                    var receiver = receiverObject as IEventReceiver;

                    Resources.Pools.EventReceivers.Instance.Pool.Add(receiver);
                }
            }
        }
    }
}
