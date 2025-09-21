using System;

namespace Daisy.Resources.Interfaces
{
    /// <summary>
    /// Defines the contract for Daisy service components that require dependency injection initialization.
    /// This interface serves as an identifier for services within the Daisy orchestration engine
    /// and provides a standardized way to initialize services with the application's service provider.
    /// Services implementing this interface can access dependency injection capabilities.
    /// </summary>
    public interface IDaisyService
    {
        /// <summary>
        /// Initializes the service with the provided service provider for dependency injection.
        /// This method is called during the service registration and startup process to provide
        /// access to the application's dependency injection container and configured services.
        /// </summary>
        /// <param name="serviceProvider">The service provider containing registered dependencies</param>
        void Initialize(IServiceProvider serviceProvider);
    }
}