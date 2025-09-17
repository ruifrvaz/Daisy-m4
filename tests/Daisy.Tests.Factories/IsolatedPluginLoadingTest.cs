using Daisy.Abilities.Weather.Services;
using Daisy.Resources.Interfaces;
using Daisy.Resources.Models;
using Daisy.Resources.Services;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Loader;

namespace Daisy.Tests.Factory.Path
{
    [TestClass]
    public class IsolatedPluginLoadingTest
    {
        private static ApplicationSettings _settings;

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            _settings = StartupFactory.AppSettingsConfiguration.LoadApplicationSettings();
        }

        [TestMethod]
        public void isolated_plugin_loading_should_maintain_type_identity()
        {
            // Create a new isolated test configuration that only loads weather as a plugin
            var isolatedSettings = new ApplicationSettings
            {
                Abilities = new List<string> { "Daisy.Abilities.Weather" },
                Receivers = new Dictionary<string, object>(),
                Transmitters = new List<string>()
            };

            // Clear any existing state
            ServiceContainer.Instance.CleanupServiceProvider();

            // Load services using the isolated settings
            var serviceProvider = StartupFactory.LoadServices(isolatedSettings);

            // Check that we got services
            var daisyServices = ServiceContainer.Instance.Services;
            Console.WriteLine($"Number of services loaded: {daisyServices.Count}");

            // Test the critical requirement: can we get the weather service?
            var weatherService = ServiceContainer.Instance.GetService<IWeatherService>();
            Console.WriteLine($"GetService<IWeatherService>() result: {weatherService}");

            // This should not be null if type identity is preserved
            weatherService.Should().NotBeNull("Weather service should be found even when loaded only as plugin");

            // Cleanup
            ServiceContainer.Instance.CleanupServiceProvider();
        }
    }
}