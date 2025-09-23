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
using System.Reflection;

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
                            Assembly.LoadFrom(mainDll);
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
        public void transmitters_are_being_loaded_into_the_pool()
        {
            TransmitterFactory.LoadExternalTransmitters(Settings!, ServiceProvider!);

            // Discover expected transmitter types using the new PluginService
            var transmitterTypes = PluginService.DiscoverTypes<IExternalTransmitter>(Settings!.Transmitters).ToList();

            transmitterTypes.Should().NotBeEmpty();

            var transmitters = Resources.Pools.ExternalTransmitters.Instance.Pool.Select(p => p.GetType().Name);

            transmitters.Count().Should().BeGreaterThanOrEqualTo(transmitterTypes.Count());
            transmitters.Except(transmitterTypes.Select(tt => tt.Name).ToList()).Should().BeEmpty();
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            ServiceContainer.Instance.CleanupServiceProvider();
        }
    }
}