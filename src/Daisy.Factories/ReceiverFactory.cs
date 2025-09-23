using Daisy.Resources.Interfaces;
using Daisy.Resources.Models;
using Daisy.Resources.Services;
using System;
using System.Linq;

namespace Daisy.Factories
{
    public sealed class ReceiverFactory
    {
        public static void LoadExternalReceivers(ApplicationSettings settings, IServiceProvider serviceProvider)
        {
            // Discover all types that implement IExternalReceiver from loaded assemblies
            var receiverTypes = PluginService.DiscoverTypes<IExternalReceiver>(settings.Receivers.Keys);

            foreach (var receiverType in receiverTypes)
            {
                // Get settings for this receiver's assembly
                var assemblyName = receiverType.Assembly.GetName().Name!;
                if (settings.Receivers.TryGetValue(assemblyName, out var assemblySettings))
                {
                    var receiver = PluginService.CreateInstance<IExternalReceiver>(receiverType, assemblySettings.RunOnCores);
                    if (receiver != null)
                    {
                        Resources.Pools.ExternalReceivers.Instance.Pool.Add(receiver);
                    }
                }
            }
        }

        public static void LoadLoopBackReceivers(ApplicationSettings settings, IServiceProvider serviceProvider)
        {
            // Discover all types that implement ILoopBackReceiver from loaded assemblies
            var receiverTypes = PluginService.DiscoverTypes<ILoopBackReceiver>(settings.Receivers.Keys);

            foreach (var receiverType in receiverTypes)
            {
                var receiver = PluginService.CreateInstance<ILoopBackReceiver>(receiverType);
                if (receiver != null)
                {
                    Resources.Pools.LoopBackReceivers.Instance.Pool.Add(receiver);
                }
            }
        }

        public static void LoadEventReceivers(ApplicationSettings settings, IServiceProvider serviceProvider)
        {
            // Discover all types that implement IEventReceiver from loaded assemblies
            var receiverTypes = PluginService.DiscoverTypes<IEventReceiver>(settings.Receivers.Keys);

            foreach (var receiverType in receiverTypes)
            {
                // Get settings for this receiver's assembly
                var assemblyName = receiverType.Assembly.GetName().Name!;
                if (settings.Receivers.TryGetValue(assemblyName, out var assemblySettings))
                {
                    var receiver = PluginService.CreateInstance<IEventReceiver>(receiverType, assemblySettings.RunOnCores);
                    if (receiver != null)
                    {
                        Resources.Pools.EventReceivers.Instance.Pool.Add(receiver);
                    }
                }
            }
        }
    }
}
