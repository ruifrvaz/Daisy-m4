using Daisy.Resources.Interfaces;
using Daisy.Resources.Models;
using Daisy.Resources.Services;
using Daisy.Resources.Startup;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
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

            var configuredAssemblies = new List<string>();
            configuredAssemblies.AddRange(settings.Abilities);
            configuredAssemblies.AddRange(settings.Receivers.Keys);
            configuredAssemblies.AddRange(settings.Transmitters);

            // Load plugin assemblies into AppDomain first
            LoadPluginAssemblies(configuredAssemblies);

            // Discover service types using the new PluginService
            var serviceTypes = PluginService.DiscoverTypes<IDaisyService>(configuredAssemblies);

            foreach (var serviceType in serviceTypes)
            {
                var service = PluginService.CreateInstance<IDaisyService>(serviceType, settings);
                if (service != null)
                {
                    ServiceContainer.Instance.Services.Add(service);
                }
            }

            var serviceProvider = serviceCollection.BuildServiceProvider();
            ServiceContainer.Instance.Services.ForEach(s => s.Initialize(serviceProvider));
            ServiceContainer.Instance.AddServiceProvider(serviceProvider);

            return serviceProvider;
        }

        private static void LoadPluginAssemblies(IEnumerable<string> configuredAssemblies)
        {
            var pluginsRoot = Path.Combine(AppContext.BaseDirectory, "plugins");
            if (!Directory.Exists(pluginsRoot))
                throw new Exception("Error loading assemblies: plugins folder not found.");

            foreach (var assemblyName in configuredAssemblies)
            {
                var pluginDir = Path.Combine(pluginsRoot, assemblyName);
                if (Directory.Exists(pluginDir))
                {
                    var mainDll = Path.Combine(pluginDir, $"{assemblyName}.dll");
                    if (File.Exists(mainDll))
                    {
                        try
                        {
                            Assembly.LoadFrom(mainDll);
                        }
                        catch
                        {
                            // Ignore load failures - some assemblies may already be loaded
                        }
                    }
                }
            }
        }
    }
}