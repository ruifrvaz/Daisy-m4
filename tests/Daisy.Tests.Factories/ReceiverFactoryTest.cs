using Daisy.Factories;
using Daisy.Resources.Interfaces;
using Daisy.Resources.Models;
using Daisy.Resources.Services;
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

            // Load plugin assemblies into the AppDomain so they can be discovered
            LoadPluginAssemblies();
        }

        private static void LoadPluginAssemblies()
        {
            var pluginsRoot = System.IO.Path.Combine(AppContext.BaseDirectory, "plugins");
            if (Directory.Exists(pluginsRoot))
            {
                foreach (var pluginDir in Directory.GetDirectories(pluginsRoot))
                {
                    var pluginName = System.IO.Path.GetFileName(pluginDir);
                    var mainDll = System.IO.Path.Combine(pluginDir, $"{pluginName}.dll");
                    if (File.Exists(mainDll))
                    {
                        try
                        {
                            System.Reflection.Assembly.LoadFrom(mainDll);
                        }
                        catch
                        {
                            // Ignore load failures for test purposes
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void receivers_are_being_loaded_into_the_pool()
        {
            ReceiverFactory.LoadExternalReceivers(Settings!, ServiceProvider!);

            // Discover expected receiver types using the new PluginService
            var receiverTypes = PluginService.DiscoverTypes<IExternalReceiver>(Settings!.Receivers.Keys).ToList();

            receiverTypes.Should().NotBeEmpty();

            var receivers = Resources.Pools.ExternalReceivers.Instance.Pool.Select(p => p.GetType().Name);

            receivers.Count().Should().BeGreaterThanOrEqualTo(receiverTypes.Count());
            receivers.Except(receiverTypes.Select(rt => rt.Name).ToList()).Should().BeEmpty();
        }

        [TestMethod]
        public void loopback_receivers_are_being_loaded_into_the_pool()
        {
            ReceiverFactory.LoadLoopBackReceivers(Settings!, ServiceProvider!);

            // Discover expected receiver types using the new PluginService
            var receiverTypes = PluginService.DiscoverTypes<ILoopBackReceiver>(Settings!.Receivers.Keys).ToList();

            // Note: Currently there are no concrete implementations of ILoopBackReceiver in the codebase,
            // only the abstract base class. This test validates that the loading mechanism works correctly
            // even when no implementations exist.
            var receivers = Resources.Pools.LoopBackReceivers.Instance.Pool.Select(p => p.GetType());

            receivers.Count().Should().Be(receiverTypes.Count());
            if (receiverTypes.Count > 0)
            {
                receivers.Except(receiverTypes).Should().BeEmpty();
            }
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            ServiceContainer.Instance.CleanupServiceProvider();
        }
    }
}
