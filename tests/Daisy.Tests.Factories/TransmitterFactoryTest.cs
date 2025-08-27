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
        private static IServiceProvider ServiceProvider;
        static ApplicationSettings Settings { get; set; }

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
            TransmitterFactory.LoadExternalTransmitters(Settings, ServiceProvider);


            var transmitterInterface = typeof(IExternalTransmitter);
            var pluginsRoot = System.IO.Path.Combine(AppContext.BaseDirectory, "plugins");
            var transmitterAssemblies = Directory.Exists(pluginsRoot)
                ? AssemblyModulesLoader.LoadFromPluginsFolder(pluginsRoot, Settings.Transmitters).ToList() : throw new Exception("Error loading modules: plugins folder not found.");

            transmitterAssemblies.Should().NotBeEmpty();

            var transmitterTypes = new List<Type>();
            foreach (var assembly in transmitterAssemblies)
            {
                transmitterTypes.AddRange(assembly.GetTypes().Where(type => transmitterInterface.IsAssignableFrom(type) && type.IsClass));
            }

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