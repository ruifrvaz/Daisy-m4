using Daisy.Abilities.Weather.Services;
using Daisy.Resources.Models;
using Daisy.Resources.Services;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace Daisy.Tests.Factory.Path
{
    [TestClass]
    public class WeatherServiceTest
    {
        private static ApplicationSettings _settings;

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            _settings = StartupFactory.AppSettingsConfiguration.LoadApplicationSettings();
        }

        [TestMethod]
        public void typeof_IWeatherService_should_not_return_null()
        {
            // Clean up any previous state
            ServiceContainer.Instance.CleanupServiceProvider();
            
            // Load services first
            var serviceProvider = StartupFactory.LoadServices(_settings);

            // This demonstrates the issue: typeof(IWeatherService) should not return null
            var weatherServiceType = typeof(IWeatherService);
            weatherServiceType.Should().NotBeNull("IWeatherService type should be available");

            // Test GetService method that relies on typeof
            var weatherService = ServiceContainer.Instance.GetService<IWeatherService>();
            weatherService.Should().NotBeNull("Weather service should be found in the service container");
        }

        [TestMethod]
        public void weather_service_should_be_loaded_as_daisy_service()
        {
            // Clean up any previous state
            ServiceContainer.Instance.CleanupServiceProvider();
            
            // Load services first
            var serviceProvider = StartupFactory.LoadServices(_settings);

            // Check that WeatherService was loaded as an IDaisyService
            var daisyServices = ServiceContainer.Instance.Services;
            daisyServices.Should().NotBeEmpty("Services should be loaded");

            var weatherServiceExists = daisyServices.Any(s => s is IWeatherService);
            weatherServiceExists.Should().BeTrue("WeatherService should be loaded as a service");
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            ServiceContainer.Instance.CleanupServiceProvider();
        }
    }
}