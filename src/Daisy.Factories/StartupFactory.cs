using Daisy.Resources.Interfaces;
using Daisy.Resources.Models;
using Daisy.Resources.Services;
using Daisy.Resources.Startup;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Daisy
{
    public static class StartupFactory
    {
        private static IStartup _appSettingsConfiguration;

        public static IStartup AppSettingsConfiguration
        {
            get
            {
                if (_appSettingsConfiguration == null)
                {
                    _appSettingsConfiguration = new AppSettingsConfiguration();
                }
                return _appSettingsConfiguration;
            }
        }

        public static void LoadService(ApplicationSettings settings, IServiceProvider serviceProvider, string agentNameSpace)
        {
            var serviceInterface = typeof(IDaisyService);

            var agentAssembly = Assembly.LoadFrom($"{agentNameSpace}.dll");

            var serviceTypes = agentAssembly.GetTypes().Where(type => serviceInterface.IsAssignableFrom(type) && type.IsClass);

            foreach (var serviceType in serviceTypes)
            {
                dynamic pathObject = Activator.CreateInstance(serviceType, settings);
                var service = pathObject as IDaisyService;
                service.Initialize(serviceProvider);
                ServiceContainer.Instance.Services.Add(service);
            }
        }

        public static IServiceProvider LoadServices(ApplicationSettings settings)
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddMemoryCache();
            serviceCollection.AddHttpClient("DefaultClient", c =>
            {
                c.DefaultRequestHeaders.Add("Accept", "application/json");
            });

            // Register IDaisyService implementations directly from loaded assemblies
            RegisterServicesFromLoadedAssemblies(serviceCollection, settings);

            var serviceProvider = serviceCollection.BuildServiceProvider();
            ServiceContainer.Instance.Services.ForEach(s => s.Initialize(serviceProvider));
            ServiceContainer.Instance.AddServiceProvider(serviceProvider);

            return serviceProvider;
        }

        private static void RegisterServicesFromLoadedAssemblies(IServiceCollection serviceCollection, ApplicationSettings settings)
        {
            var serviceInterface = typeof(IDaisyService);

            // Get all loaded assemblies that contain plugin types
            var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies()
                .Where(assembly => assembly.GetName().Name.StartsWith("Daisy."))
                .ToList();

            foreach (var assembly in loadedAssemblies)
            {
                var serviceTypes = assembly.GetTypes()
                    .Where(type => serviceInterface.IsAssignableFrom(type) && type.IsClass && !type.IsAbstract);

                foreach (var serviceType in serviceTypes)
                {
                    try
                    {
                        var service = (IDaisyService)Activator.CreateInstance(serviceType, settings);
                        ServiceContainer.Instance.Services.Add(service);
                    }
                    catch (Exception ex)
                    {
                        // Log the error but continue loading other services
                        Console.WriteLine($"Failed to register service {serviceType.Name}: {ex.Message}");
                    }
                }
            }
        }
    }
}