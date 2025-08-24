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
            // look in the assembly and find all classes that implement IExternalTransmitter
            var transmitterInterface = typeof(IExternalTransmitter);
            foreach (var transmitterAssemblyName in settings.Transmitters)
            {
                var assembly = Assembly.Load(transmitterAssemblyName);
                var transmitterTypes = assembly.GetTypes().Where(type => transmitterInterface.IsAssignableFrom(type) && type.IsClass).ToList();
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
        }

        public static void LoadLoopBackTransmitters(ApplicationSettings settings, IServiceProvider serviceProvider)
        {
            // look in the assembly and find all classes that implement ILoopBackTransmitter
            var transmitterInterface = typeof(ILoopBackTransmitter);
            foreach (var transmitterAssemblyName in settings.Transmitters)
            {
                var assembly = Assembly.Load(transmitterAssemblyName);
                var receiverTypes = assembly.GetTypes().Where(type => transmitterInterface.IsAssignableFrom(type) && type.IsClass).ToList();
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
        }
    }
}