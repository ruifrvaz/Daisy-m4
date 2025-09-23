using Daisy.Resources.Interfaces;
using Daisy.Resources.Models;
using Daisy.Resources.Pools;
using Daisy.Resources.Services;
using System;
using System.Linq;

namespace Daisy.Factories
{
    public sealed class TransmitterFactory
    {
        public static void LoadExternalTransmitters(ApplicationSettings settings, IServiceProvider serviceProvider)
        {
            // Discover all types that implement IExternalTransmitter from loaded assemblies
            var transmitterTypes = PluginService.DiscoverTypes<IExternalTransmitter>(settings.Transmitters);

            foreach (var transmitterType in transmitterTypes)
            {
                var transmitter = PluginService.CreateInstance<IExternalTransmitter>(transmitterType);
                if (transmitter != null)
                {
                    ExternalTransmitters.Instance.Pool.Add(transmitter);
                }
            }
        }

        public static void LoadLoopBackTransmitters(ApplicationSettings settings, IServiceProvider serviceProvider)
        {
            // Discover all types that implement ILoopBackTransmitter from loaded assemblies
            var transmitterTypes = PluginService.DiscoverTypes<ILoopBackTransmitter>(settings.Transmitters);

            foreach (var transmitterType in transmitterTypes)
            {
                var transmitter = PluginService.CreateInstance<ILoopBackTransmitter>(transmitterType);
                if (transmitter != null)
                {
                    LoopBackTransmitters.Instance.Pool.Add(transmitter);
                }
            }
        }
    }
}