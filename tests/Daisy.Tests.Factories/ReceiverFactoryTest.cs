using Daisy.Factories;
using Daisy.Resources.Interfaces;
using Daisy.Resources.Models;
using Daisy.Resources.Services;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
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
            // Force loading of receiver assemblies by referencing types from them
            _ = typeof(Daisy.Receivers.Console.ConsoleReceiver);
            _ = typeof(Daisy.Receivers.WeatherEvent.WeatherEventReceiver);
            
            ReceiverFactory.LoadExternalReceivers(Settings!, ServiceProvider!);

            var receiverInterface = typeof(IExternalReceiver);

            // Use assembly scanning approach instead of plugin loading
            var receiverAssemblies = AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => !a.IsDynamic && a.GetName().Name.StartsWith("Daisy.Receivers."))
                .ToList();

            receiverAssemblies.Should().NotBeEmpty();

            var receiverTypes = new List<Type>();
            foreach (var assembly in receiverAssemblies)
            {
                receiverTypes.AddRange(assembly.GetTypes()
                    .Where(type => receiverInterface.IsAssignableFrom(type) 
                                  && type.IsClass 
                                  && !type.IsAbstract));
            }

            var receivers = Resources.Pools.ExternalReceivers.Instance.Pool.Select(p => p.GetType().Name);

            receivers.Count().Should().BeGreaterThanOrEqualTo(receiverTypes.Count());
            receivers.Except(receiverTypes.Select(rt => rt.Name).ToList()).Should().BeEmpty();
        }

        [TestMethod]
        public void loopback_receivers_are_being_loaded_into_the_pool()
        {
            // Force loading of receiver assemblies by referencing types from them
            _ = typeof(Daisy.Receivers.Console.ConsoleReceiver);
            _ = typeof(Daisy.Receivers.WeatherEvent.WeatherEventReceiver);
            
            ReceiverFactory.LoadLoopBackReceivers(Settings!, ServiceProvider!);

            var receiverInterface = typeof(ILoopBackReceiver);

            // Use assembly scanning approach instead of plugin loading
            var receiverAssemblies = AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => !a.IsDynamic && a.GetName().Name.StartsWith("Daisy.Receivers."))
                .ToList();

            receiverAssemblies.Should().NotBeEmpty();

            var receiverTypes = new List<Type>();
            foreach (var assembly in receiverAssemblies)
            {
                receiverTypes.AddRange(assembly.GetTypes()
                    .Where(type => receiverInterface.IsAssignableFrom(type) 
                                  && type.IsClass 
                                  && !type.IsAbstract));
            }

            var receivers = Resources.Pools.LoopBackReceivers.Instance.Pool.Select(p => p.GetType());

            receivers.Count().Should().BeGreaterThanOrEqualTo(receiverTypes.Count());
            receivers.Except(receiverTypes).Should().BeEmpty();
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            ServiceContainer.Instance.CleanupServiceProvider();
        }
    }
}
