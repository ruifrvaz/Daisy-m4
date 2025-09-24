using Daisy.Factories;
using Daisy.Resources.Interfaces;
using Daisy.Resources.Models;
using Daisy.Resources.Services;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
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
            // Force loading of transmitter assemblies by referencing types from them
            _ = typeof(Daisy.Transmitters.Console.ConsoleTransmitter);
            _ = typeof(Daisy.Transmitters.WorkflowTrigger.WorkflowTriggerLoopbackTransmitter);
            
            TransmitterFactory.LoadExternalTransmitters(Settings!, ServiceProvider!);

            var transmitterInterface = typeof(IExternalTransmitter);
            
            // Use assembly scanning approach instead of plugin loading
            var transmitterAssemblies = AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => !a.IsDynamic && a.GetName().Name.StartsWith("Daisy.Transmitters."))
                .ToList();

            transmitterAssemblies.Should().NotBeEmpty();

            var transmitterTypes = new List<Type>();
            foreach (var assembly in transmitterAssemblies)
            {
                transmitterTypes.AddRange(assembly.GetTypes()
                    .Where(type => transmitterInterface.IsAssignableFrom(type) 
                                  && type.IsClass 
                                  && !type.IsAbstract));
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