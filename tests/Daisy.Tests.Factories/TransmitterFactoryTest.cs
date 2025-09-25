using Daisy.Factories;
using Daisy.Resources.Interfaces;
using Daisy.Resources.Models;
using Daisy.Resources.Services;
using Daisy.Resources.Startup;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Daisy.Tests.Factory.Transmitter
{
    [TestClass]
    public class TransmitterFactoryTest
    {
        private static IServiceProvider? ServiceProvider;
        static ApplicationSettings? Settings { get; set; }

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            Settings = new ApplicationSettings
            {
                Transmitters = new List<string>() { "Daisy.Transmitters.Console" },
                Abilities = new List<string>()
            };
            ServiceProvider = StartupFactory.LoadServices(Settings);
        }

        [TestMethod]
        public void transmitters_are_being_loaded_into_the_pool()
        {
            TransmitterFactory.LoadExternalTransmitters(Settings!, ServiceProvider!);

            // Check that expected transmitter types were loaded directly
            var expectedTransmitterTypes = new[]
            {
                typeof(Daisy.Transmitters.Console.ConsoleTransmitter),
                // WorkflowTriggerLoopbackTransmitter implements ILoopBackTransmitter, not IExternalTransmitter
            }.Where(t => typeof(IExternalTransmitter).IsAssignableFrom(t)).ToList();

            var transmitters = Resources.Pools.ExternalTransmitters.Instance.Pool.Select(p => p.GetType());

            transmitters.Count().Should().BeGreaterThanOrEqualTo(expectedTransmitterTypes.Count);

            foreach (var expectedType in expectedTransmitterTypes)
            {
                transmitters.Should().Contain(t => t == expectedType);
            }
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            ServiceContainer.Instance.CleanupServiceProvider();
        }
    }
}