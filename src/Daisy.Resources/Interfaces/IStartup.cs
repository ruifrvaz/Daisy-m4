using Daisy.Resources.Models;
using Microsoft.Extensions.Configuration;

namespace Daisy.Resources.Interfaces
{
    /// <summary>
    /// Defines the contract for application startup and configuration management in the Daisy engine.
    /// Startup components handle the loading and saving of application settings, providing a standardized
    /// way to manage configuration data during application initialization and runtime configuration changes.
    /// This interface enables consistent configuration management across the workflow orchestration system.
    /// </summary>
    public interface IStartup
    {
        /// <summary>
        /// Loads the application settings from the configured source.
        /// This method retrieves the current application configuration including
        /// module settings, API configurations, and workflow parameters.
        /// </summary>
        /// <returns>The loaded application settings object</returns>
        ApplicationSettings LoadApplicationSettings();

        /// <summary>
        /// Saves the provided application settings to the configured destination.
        /// This method persists configuration changes made during runtime,
        /// ensuring settings are maintained across application restarts.
        /// </summary>
        /// <param name="settings">The application settings to save</param>
        void SaveApplicationSettings(ApplicationSettings settings);
    }
}