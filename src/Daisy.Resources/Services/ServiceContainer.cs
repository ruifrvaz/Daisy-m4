using Daisy.Resources.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Daisy.Resources.Services
{
    /// <summary>
    /// Singleton service container that manages Daisy service instances and the application's service provider.
    /// This class provides centralized service management, registration, and dependency injection capabilities
    /// for the Daisy workflow orchestration engine. It maintains the service provider lifecycle and enables
    /// service discovery and cleanup operations.
    /// </summary>
    public sealed class ServiceContainer
    {
        /// <summary>
        /// Gets or sets the collection of registered Daisy service instances.
        /// Contains all services that implement IDaisyService and are available
        /// for dependency injection and workflow operations.
        /// </summary>
        public List<IDaisyService> Services { get; set; }

        /// <summary>
        /// Holds the application's service provider for dependency injection.
        /// </summary>
        private static ServiceProvider _serviceProvider;

        /// <summary>
        /// Holds the singleton instance of the ServiceContainer.
        /// </summary>
        private static ServiceContainer _instance;

        /// <summary>
        /// Initializes a new instance of the ServiceContainer class.
        /// Private constructor to enforce singleton pattern and prevent external instantiation.
        /// Creates an empty list of services ready for registration.
        /// </summary>
        private ServiceContainer()
        {
            Services = new List<IDaisyService>();
        }

        /// <summary>
        /// Gets the singleton instance of the ServiceContainer.
        /// Creates the instance on first access if it doesn't exist (lazy initialization).
        /// Thread-safe singleton implementation for managing the service container.
        /// </summary>
        public static ServiceContainer Instance
        {
            get
            {
                return _instance ?? (_instance = new ServiceContainer());
            }
        }

        /// <summary>
        /// Retrieves a service of the specified type from the registered services collection.
        /// Searches through registered services to find the first instance that matches
        /// or is assignable to the specified service type.
        /// </summary>
        /// <typeparam name="T">The type of service to retrieve, must implement IDaisyService</typeparam>
        /// <returns>The first matching service instance, or null if no match is found</returns>
        public IDaisyService GetService<T>() where T : IDaisyService
        {
            return Instance.Services.FirstOrDefault(s => typeof(T).IsAssignableFrom(s.GetType()));
        }

        /// <summary>
        /// Adds or updates the service provider for dependency injection.
        /// Sets the service provider only if it hasn't been initialized yet,
        /// ensuring singleton behavior for the service provider instance.
        /// </summary>
        /// <param name="serviceProvider">The service provider to register</param>
        public void AddServiceProvider(ServiceProvider serviceProvider)
        {
            _serviceProvider ??= serviceProvider;
        }

        /// <summary>
        /// Retrieves the current service provider instance.
        /// Provides access to the dependency injection container for service resolution.
        /// </summary>
        /// <returns>The registered service provider</returns>
        /// <exception cref="Exception">Thrown when the service provider has not been initialized</exception>
        public ServiceProvider GetServiceProvider()
        {
            if (_serviceProvider == null)
            {
                throw new Exception("Service provider not initialized.");
            }

            return _serviceProvider;
        }

        /// <summary>
        /// Performs cleanup of the service provider and all registered services.
        /// This method:
        /// 1. Disposes all services that implement IDisposable
        /// 2. Clears the services collection
        /// 3. Disposes the service provider
        /// 4. Resets the service provider to null
        /// 
        /// Should be called during application shutdown to ensure proper resource cleanup.
        /// </summary>
        public void CleanupServiceProvider()
        {
            if (_serviceProvider != null)
            {
                foreach (var disposable in Services.OfType<IDisposable>())
                {
                    disposable.Dispose();
                }
                Services.Clear();

                _serviceProvider.Dispose();
                _serviceProvider = null;
            }
        }
    }
}
