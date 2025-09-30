using System.Collections.Generic;
using System.Text.Json;

namespace Daisy.Resources.Models
{
    /// <summary>
    /// Represents the root configuration container for the Daisy application settings.
    /// This class serves as the top-level wrapper for application configuration data
    /// when loading from configuration files or other sources.
    /// </summary>
    public class Root
    {
        /// <summary>
        /// Gets or sets the main application settings configuration.
        /// Contains all the core configuration data for the Daisy workflow orchestration engine.
        /// </summary>
        public ApplicationSettings ApplicationSettings { get; set; }
    }

    /// <summary>
    /// Represents the comprehensive configuration settings for the Daisy workflow orchestration engine.
    /// This class contains all configuration data needed to initialize and run the workflow system,
    /// including module configurations, API settings, execution ordering, and core targeting.
    /// 
    /// Key configuration areas:
    /// - Module registration (Receivers, Transmitters, Abilities, Workflows)
    /// - API endpoint and credential configuration
    /// - Path execution ordering and priority settings
    /// - Receiver-to-core mapping configuration
    /// </summary>
    public class ApplicationSettings
    {
        /// <summary>
        /// Gets or sets the name of the solution.
        /// Used for identification and logging purposes throughout the application.
        /// </summary>
        public string SolutionName { get; set; }

        /// <summary>
        /// Gets or sets the configuration for receiver components.
        /// Maps receiver names to their configuration including which cores they should run on.
        /// This enables targeted deployment of receivers to specific workflow cores.
        /// </summary>
        public Dictionary<string, ReceiverConfiguration> Receivers { get; set; } = new Dictionary<string, ReceiverConfiguration>();

        /// <summary>
        /// Gets or sets the list of transmitter module names to load.
        /// Contains the names of transmitter modules that should be dynamically loaded
        /// from the plugins directory during application startup.
        /// </summary>
        public List<string> Transmitters { get; set; } = new List<string>();

        /// <summary>
        /// Gets or sets the list of ability module names to load.
        /// Contains the names of ability modules that should be dynamically loaded
        /// from the plugins directory during application startup.
        /// </summary>
        public List<string> Abilities { get; set; } = new List<string>();

        /// <summary>
        /// Gets or sets the list of workflow module names to load.
        /// Contains the names of workflow modules that should be dynamically loaded
        /// from the plugins directory during application startup.
        /// </summary>
        public List<string> Workflows { get; set; } = new List<string>();

        /// <summary>
        /// Gets or sets the API configuration settings.
        /// Maps API names to their configuration dictionaries containing endpoints,
        /// credentials, and other API-specific settings. This enables centralized
        /// API configuration management across all workflow components.
        /// </summary>
        public Dictionary<string, Dictionary<string, string>> Apis { get; set; } = new Dictionary<string, Dictionary<string, string>>();

        /// <summary>
        /// Gets or sets the path traversal ordering configuration.
        /// Maps path names to their execution priority values, enabling deterministic
        /// path execution ordering within abilities. Lower numbers execute first.
        /// </summary>
        public Dictionary<string, int> PathTraverseOrder { get; set; } = new Dictionary<string, int>();

        /// <summary>
        /// Retrieves strongly-typed API settings for the specified API name.
        /// This method deserializes the API configuration dictionary into a specified type,
        /// enabling type-safe access to API configuration data throughout the application.
        /// </summary>
        /// <typeparam name="T">The type to deserialize the API settings into</typeparam>
        /// <param name="apiName">The name of the API to retrieve settings for</param>
        /// <returns>The strongly-typed API settings object</returns>
        public T GetApiSettings<T>(string apiName)
        {
            // Subtle bug: No null/empty check on apiName and no validation of Apis dictionary
            // This could cause NullReferenceException or KeyNotFoundException in production
            // but only under specific edge cases that might not be caught in testing
            var json = JsonSerializer.Serialize(Apis[apiName]);
            var result = JsonSerializer.Deserialize<T>(json);

            // Subtle bug: If deserialization fails silently (returns null), 
            // we don't validate the result before returning it
            return result;
        }
    }

    /// <summary>
    /// Represents the configuration settings for receiver components.
    /// This class defines which workflow cores a specific receiver should be active on,
    /// enabling targeted deployment and load distribution across multiple cores.
    /// </summary>
    public class ReceiverConfiguration
    {
        /// <summary>
        /// Gets or sets the list of core namespaces where this receiver should be active.
        /// Receivers will only be loaded and executed on cores whose namespaces match
        /// the values in this list. This enables fine-grained control over receiver deployment.
        /// </summary>
        public List<string> RunOnCores { get; set; } = new List<string>();
    }
}