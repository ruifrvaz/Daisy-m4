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
        private static IServiceProvider ServiceProvider;
        private static ApplicationSettings Settings { get; set; }

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            Settings = new ApplicationSettings
            {
                Receivers = new List<string>() { "Daisy.Receivers.Console", "Daisy.Receivers.Event" },
                Abilities = new List<string>()
            };
            ServiceProvider = StartupFactory.LoadServices(Settings);
        }

        [TestMethod]
        public void receivers_are_being_loaded_into_the_pool()
        {
            ReceiverFactory.LoadExternalReceivers(Settings, ServiceProvider);

            var receiverInterface = typeof(IExternalReceiver);
            var receiverAssemblies = AppDomain.CurrentDomain.GetAssemblies().Where(ass => ass.GetName().Name.StartsWith("Daisy.Receivers")).ToList();
            receiverAssemblies.Should().NotBeEmpty();

            var receiverTypes = new List<Type>();
            foreach (var assembly in receiverAssemblies)
            {
                receiverTypes.AddRange(assembly.GetTypes().Where(type => receiverInterface.IsAssignableFrom(type) && type.IsClass));
            }

            var receivers = Resources.Pools.ExternalReceivers.Instance.Pool.Select(p => p.GetType());

            receivers.Count().Should().BeGreaterThanOrEqualTo(receiverTypes.Count());
            receivers.Except(receiverTypes).Should().BeEmpty();
        }

        [TestMethod]
        public void loopback_receivers_are_being_loaded_into_the_pool()
        {
            ReceiverFactory.LoadLoopBackReceivers(Settings, ServiceProvider);

            var receiverInterface = typeof(ILoopBackReceiver);
            var receiverAssemblies = AppDomain.CurrentDomain.GetAssemblies().Where(ass => ass.GetName().Name.StartsWith("Daisy.Receivers")).ToList();
            receiverAssemblies.Should().NotBeEmpty();

            var receiverTypes = new List<Type>();
            foreach (var assembly in receiverAssemblies)
            {
                receiverTypes.AddRange(assembly.GetTypes().Where(type => receiverInterface.IsAssignableFrom(type) && type.IsClass));
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
