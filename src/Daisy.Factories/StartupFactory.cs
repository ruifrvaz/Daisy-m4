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

        public static IServiceProvider LoadServices(ApplicationSettings settings)
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddMemoryCache();
            serviceCollection.AddHttpClient("DefaultClient", c =>
            {
                c.DefaultRequestHeaders.Add("Accept", "application/json");
            });

            // Register all IDaisyService implementations from loaded assemblies
            RegisterDaisyServices(serviceCollection, settings);

            var serviceProvider = serviceCollection.BuildServiceProvider();
            
            // Initialize all registered services
            ServiceContainer.Instance.Services.ForEach(s => s.Initialize(serviceProvider));
            ServiceContainer.Instance.AddServiceProvider(serviceProvider);

            return serviceProvider;
        }

        private static void RegisterDaisyServices(IServiceCollection services, ApplicationSettings settings)
        {
            var serviceInterface = typeof(IDaisyService);
            
            // Get all loaded assemblies that might contain IDaisyService implementations
            var assemblies = AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => !a.IsDynamic && a.GetName().Name.StartsWith("Daisy."))
                .ToList();

            foreach (var assembly in assemblies)
            {
                try
                {
                    var serviceTypes = assembly.GetTypes()
                        .Where(type => serviceInterface.IsAssignableFrom(type) 
                                      && type.IsClass 
                                      && !type.IsAbstract)
                        .ToList();

                    foreach (var serviceType in serviceTypes)
                    {
                        // Register the service in DI container
                        services.AddTransient(serviceInterface, serviceType);
                        
                        // Also create instances for backwards compatibility with ServiceContainer
                        try
                        {
                            var instance = Activator.CreateInstance(serviceType, settings);
                            if (instance is IDaisyService service)
                            {
                                ServiceContainer.Instance.Services.Add(service);
                            }
                        }
                        catch (Exception ex)
                        {
                            // Log or handle creation errors gracefully
                            Console.WriteLine($"Warning: Could not instantiate service {serviceType.Name}: {ex.Message}");
                        }
                    }
                }
                catch (ReflectionTypeLoadException ex)
                {
                    // Handle assemblies that can't be fully loaded
                    Console.WriteLine($"Warning: Could not load all types from assembly {assembly.GetName().Name}: {ex.Message}");
                }
            }
        }
    }
}