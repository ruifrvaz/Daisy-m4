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

            ServiceProvider = StartupFactory.LoadServices(Settings);
        }

        [TestMethod]
        public void load_only_paths_that_are_in_traverse_settings()
        {
            // Force loading of ability assemblies by referencing types from them
            _ = typeof(Daisy.Abilities.Operator.ListWorkflowsPath);
            _ = typeof(Daisy.Abilities.OutputValidator.Paths.OutputValidatorPath);
            _ = typeof(Daisy.Abilities.Terminate.TerminatePath);
            _ = typeof(Daisy.Abilities.Weather.Paths.GetWeatherByCityPath);
            
            AbilityFactory.LoadAbilities(Settings!, ServiceProvider!);

            var pathInterface = typeof(IPath);
            
            // With direct assembly scanning, we check loaded assemblies that start with Daisy.Abilities.
            var abilities = AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => !a.IsDynamic && a.GetName().Name.StartsWith("Daisy.Abilities."))
                .ToList();
            abilities.Should().NotBeEmpty();

            var pathTypes = new List<Type>();
            foreach (var ability in abilities)
            {
                pathTypes.AddRange(ability.GetExportedTypes()
                    .Where(type => pathInterface.IsAssignableFrom(type) 
                                  && type.IsClass 
                                  && !type.IsAbstract
                                  && Settings!.PathTraverseOrder.ContainsKey(type.Name)));
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