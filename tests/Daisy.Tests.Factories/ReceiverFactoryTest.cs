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

namespace Daisy.Tests.Factory.Receiver
{
    [TestClass]
    public class ReceiverFactoryTest
    {
        private static IServiceProvider? ServiceProvider;
        private static ApplicationSettings? Settings { get; set; }

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            Settings = new ApplicationSettings
            {
                Receivers = new Dictionary<string, ReceiverConfiguration>()
                {
                    { "Daisy.Receivers.Console", new ReceiverConfiguration() },
                    { "Daisy.Receivers.Event", new ReceiverConfiguration() }
                },
                Abilities = new List<string>()
            };
            ServiceProvider = StartupFactory.LoadServices(Settings);
        }

        [TestMethod]
        public void receivers_are_being_loaded_into_the_pool()
        {
            ReceiverFactory.LoadExternalReceivers(Settings!, ServiceProvider!);

            // Check that expected receiver types were loaded directly
            var expectedReceiverTypes = new[]
            {
                typeof(Daisy.Receivers.Console.ConsoleReceiver),
                // WeatherEventReceiver doesn't implement IExternalReceiver, so it won't be in this pool
            }.Where(t => typeof(IExternalReceiver).IsAssignableFrom(t)).ToList();

            var receivers = Resources.Pools.ExternalReceivers.Instance.Pool.Select(p => p.GetType());

            receivers.Count().Should().BeGreaterThanOrEqualTo(expectedReceiverTypes.Count);
            
            foreach (var expectedType in expectedReceiverTypes)
            {
                receivers.Should().Contain(r => r == expectedType);
            }
        }

        [TestMethod]
        public void loopback_receivers_are_being_loaded_into_the_pool()
        {
            ReceiverFactory.LoadLoopBackReceivers(Settings!, ServiceProvider!);

            // Check if any of our known receivers implement ILoopBackReceiver
            var expectedReceiverTypes = new[]
            {
                typeof(Daisy.Receivers.Console.ConsoleReceiver),
                typeof(Daisy.Receivers.WeatherEvent.WeatherEventReceiver)
            }.Where(t => typeof(ILoopBackReceiver).IsAssignableFrom(t)).ToList();

            var receivers = Resources.Pools.LoopBackReceivers.Instance.Pool.Select(p => p.GetType());

            // Since we only load types that actually implement the interface, 
            // the count should match exactly
            receivers.Count().Should().Be(expectedReceiverTypes.Count);
            
            foreach (var expectedType in expectedReceiverTypes)
            {
                receivers.Should().Contain(r => r == expectedType);
            }
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            ServiceContainer.Instance.CleanupServiceProvider();
        }
    }
}
