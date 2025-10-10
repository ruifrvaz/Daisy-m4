using Daisy.Factories;
using Daisy.Resources.Interfaces;
using Daisy.Resources.Models;
using Daisy.Resources.Pools;
using Daisy.Resources.Services;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Daisy.Tests.Factory.Path
{
    [TestClass]
    public class AbilityFactoryTest
    {
        private static IServiceProvider? ServiceProvider;
        static ApplicationSettings? Settings { get; set; }

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            Settings = StartupFactory.AppSettingsConfiguration.LoadApplicationSettings();

            // Force load ability assemblies by referencing types from each
            var operatorType = typeof(Daisy.Abilities.Operator.ListWorkflowsPath);
            var terminateType = typeof(Daisy.Abilities.Terminate.TerminatePath);
            var validatorType = typeof(Daisy.Abilities.OutputValidator.Paths.OutputValidatorPath);
            var weatherType = typeof(Daisy.Abilities.Weather.Paths.GetWeatherByCityPath);
            var flightsType = typeof(Daisy.Abilities.Flights.Paths.GetFlightsByCityPath);

            ServiceProvider = StartupFactory.LoadServices(Settings);
        }

        [TestMethod]
        public void load_only_paths_that_are_in_traverse_settings()
        {
            AbilityFactory.LoadAbilities(Settings!, ServiceProvider!);

            var pathInterface = typeof(IPath);
            var abilities = AppDomain.CurrentDomain.GetAssemblies().Where(ass => Settings!.Abilities.Contains(ass.GetName().Name)).ToList();
            abilities.Should().NotBeEmpty();

            var pathTypes = new List<Type>();
            foreach (var ability in abilities)
            {
                pathTypes.AddRange(ability.GetTypes().Where(type => pathInterface.IsAssignableFrom(type) && type.IsClass));
            }

            var paths = Paths.Instance.Pool.Select(p => p.GetType());

            paths.Count().Should().Be(Settings!.PathTraverseOrder.Count());
            Settings.PathTraverseOrder.Select(to => to.Key).ToList().Should().Contain(paths.First().Name);
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            ServiceContainer.Instance.CleanupServiceProvider();
        }
    }
}