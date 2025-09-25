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
            // Load known external transmitter types directly
            LoadExternalTransmitter<Daisy.Transmitters.Console.ConsoleTransmitter>();
        }

        private static void LoadExternalTransmitter<T>() where T : class, new()
        {
            var transmitterInterface = typeof(IExternalTransmitter);
            var transmitterType = typeof(T);

            if (transmitterInterface.IsAssignableFrom(transmitterType) && transmitterType.IsClass)
            {
                var transmitter = new T() as IExternalTransmitter;
                ExternalTransmitters.Instance.Pool.Add(transmitter);
            }
        }

        public static void LoadLoopBackTransmitters(ApplicationSettings settings, IServiceProvider serviceProvider)
        {
            // Load known loopback transmitter types directly
            LoadLoopBackTransmitter<Daisy.Transmitters.WorkflowTrigger.WorkflowTriggerLoopbackTransmitter>();
        }

        private static void LoadLoopBackTransmitter<T>() where T : class, new()
        {
            var transmitterInterface = typeof(ILoopBackTransmitter);
            var transmitterType = typeof(T);

            if (transmitterInterface.IsAssignableFrom(transmitterType) && transmitterType.IsClass)
            {
                var transmitter = new T() as ILoopBackTransmitter;
                LoopBackTransmitters.Instance.Pool.Add(transmitter);
            }
        }
    }
}