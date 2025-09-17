using Daisy.Abilities.Weather.Services;
using Daisy.Resources.Interfaces;
using Daisy.Resources.Models;
using Daisy.Resources.Services;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Runtime.Loader;

namespace Daisy.Tests.Factory.Path
{
    [TestClass]
    public class AssemblyLoadingDiagnosticTest
    {
        private static ApplicationSettings _settings;

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            _settings = StartupFactory.AppSettingsConfiguration.LoadApplicationSettings();
        }

        [TestMethod]
        public void diagnose_assembly_loading_issue()
        {
            // Load services first
            var serviceProvider = StartupFactory.LoadServices(_settings);

            // Debug information
            var daisyServices = ServiceContainer.Instance.Services;
            Console.WriteLine($"Number of services loaded: {daisyServices.Count}");
            
            foreach (var service in daisyServices)
            {
                Console.WriteLine($"Service type: {service.GetType().FullName}");
                Console.WriteLine($"Assembly: {service.GetType().Assembly.FullName}");
                Console.WriteLine($"Assembly LoadContext: {AssemblyLoadContext.GetLoadContext(service.GetType().Assembly)}");
                Console.WriteLine($"Implements IDaisyService: {service is IDaisyService}");
                Console.WriteLine($"Implements IWeatherService: {service is IWeatherService}");
                Console.WriteLine("---");
            }

            // Check the type resolution problem
            var weatherServiceType = typeof(IWeatherService);
            Console.WriteLine($"IWeatherService type: {weatherServiceType.FullName}");
            Console.WriteLine($"IWeatherService assembly: {weatherServiceType.Assembly.FullName}");
            Console.WriteLine($"IWeatherService LoadContext: {AssemblyLoadContext.GetLoadContext(weatherServiceType.Assembly)}");

            // Check which assemblies are loaded in the current AppDomain
            var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            Console.WriteLine($"Loaded assemblies in AppDomain:");
            foreach (var asm in loadedAssemblies.Where(a => a.FullName.Contains("Daisy")))
            {
                Console.WriteLine($"  - {asm.FullName}");
                Console.WriteLine($"    LoadContext: {AssemblyLoadContext.GetLoadContext(asm)}");
            }

            // Test the actual issue: GetService method
            var weatherService = ServiceContainer.Instance.GetService<IWeatherService>();
            Console.WriteLine($"GetService<IWeatherService>() result: {weatherService}");

            // Let's manually test the IsAssignableFrom logic that GetService uses
            foreach (var service in daisyServices)
            {
                var serviceType = service.GetType();
                var isAssignable = typeof(IWeatherService).IsAssignableFrom(serviceType);
                Console.WriteLine($"typeof(IWeatherService).IsAssignableFrom({serviceType.Name}): {isAssignable}");
                
                // Let's also check if the interface types are actually the same
                var serviceInterfaces = serviceType.GetInterfaces();
                foreach (var intf in serviceInterfaces)
                {
                    Console.WriteLine($"  Interface: {intf.FullName} (LoadContext: {AssemblyLoadContext.GetLoadContext(intf.Assembly)})");
                    Console.WriteLine($"  Same type? {intf == typeof(IWeatherService)}");
                    Console.WriteLine($"  Same assembly? {intf.Assembly == typeof(IWeatherService).Assembly}");
                }
            }
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            ServiceContainer.Instance.CleanupServiceProvider();
        }
    }
}