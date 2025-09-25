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

            // Import specific receiver types directly - since we have direct references now
            LoadExternalReceiver<Daisy.Receivers.Console.ConsoleReceiver>(settings, receiverInterface);
            LoadExternalReceiver<Daisy.Receivers.WeatherEvent.WeatherEventReceiver>(settings, receiverInterface);
        }

        private static void LoadExternalReceiver<T>(ApplicationSettings settings, Type receiverInterface) where T : class
        {
            var receiverType = typeof(T);
            if (!receiverInterface.IsAssignableFrom(receiverType) || !receiverType.IsClass)
                return;

            var assemblyName = receiverType.Assembly.GetName().Name!;
            if (!settings.Receivers.ContainsKey(assemblyName))
                return;

            var assemblySettings = settings.Receivers[assemblyName];
            var receiver = (IExternalReceiver)Activator.CreateInstance(receiverType, assemblySettings.RunOnCores);
            Resources.Pools.ExternalReceivers.Instance.Pool.Add(receiver);
        }

        public static void LoadLoopBackReceivers(ApplicationSettings settings, IServiceProvider serviceProvider)
        {
            // find all assemblies that implement ILoopBackReceiver, create an instance for each of them
            // and load them into receiver pool
            var receiverInterface = typeof(ILoopBackReceiver);

            // Check if any of our known receiver types implement ILoopBackReceiver
            LoadLoopBackReceiver<Daisy.Receivers.Console.ConsoleReceiver>(receiverInterface);
            LoadLoopBackReceiver<Daisy.Receivers.WeatherEvent.WeatherEventReceiver>(receiverInterface);
        }

        private static void LoadLoopBackReceiver<T>(Type receiverInterface) where T : class
        {
            var receiverType = typeof(T);
            if (!receiverInterface.IsAssignableFrom(receiverType) || !receiverType.IsClass)
                return;

            var receiver = (ILoopBackReceiver)Activator.CreateInstance(receiverType);
            Resources.Pools.LoopBackReceivers.Instance.Pool.Add(receiver);
        }

        public static void LoadEventReceivers(ApplicationSettings settings, IServiceProvider serviceProvider)
        {
            // find all assemblies that implement IEventReceiver, create an instance for each of them and load them into receiver pool
            var receiverInterface = typeof(IEventReceiver);

            // Load known event receiver types directly
            LoadEventReceiver<Daisy.Receivers.Console.ConsoleReceiver>(settings, receiverInterface);
            LoadEventReceiver<Daisy.Receivers.WeatherEvent.WeatherEventReceiver>(settings, receiverInterface);
        }

        private static void LoadEventReceiver<T>(ApplicationSettings settings, Type receiverInterface) where T : class
        {
            var receiverType = typeof(T);
            if (!receiverInterface.IsAssignableFrom(receiverType) || !receiverType.IsClass)
                return;

            var assemblyName = receiverType.Assembly.GetName().Name!;
            if (!settings.Receivers.ContainsKey(assemblyName))
                return;

            var assemblySettings = settings.Receivers[assemblyName];
            var receiver = (IEventReceiver)Activator.CreateInstance(receiverType, assemblySettings.RunOnCores);
            Resources.Pools.EventReceivers.Instance.Pool.Add(receiver);
        }
    }
}
