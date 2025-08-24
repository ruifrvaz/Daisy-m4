using Daisy.Resources.Models;
using Daisy.Resources.Services;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net.Http;

namespace Daisy.Tests.Factory.Path
{
    [TestClass]
    public class StartupFactoryTest
    {
        private static ApplicationSettings _settings;

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            _settings = StartupFactory.AppSettingsConfiguration.LoadApplicationSettings();
        }

        [TestMethod]
        public void serviceProvider_should_load_only_once()
        {
            var firstServiceProvider = StartupFactory.LoadServices(_settings);

            var serviceProvider = ServiceContainer.Instance.GetServiceProvider();

            var serviceCollection = new ServiceCollection();

            var secondServiceProvider = serviceCollection.BuildServiceProvider();

            ServiceContainer.Instance.AddServiceProvider(secondServiceProvider);

            var _httpClient = serviceProvider.GetService<IHttpClientFactory>().CreateClient("DefaultClient");

            _httpClient.Should().NotBeNull();
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            ServiceContainer.Instance.CleanupServiceProvider();
        }
    }
}