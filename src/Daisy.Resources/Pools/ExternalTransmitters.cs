using Daisy.Resources.Interfaces;
using System.Collections.Generic;

namespace Daisy.Resources.Pools
{
    /// <summary>
    /// Singleton pool that manages all external transmitter instances in the Daisy workflow engine.
    /// External transmitters handle the final output stage of workflow processing, sending results
    /// to external destinations such as APIs, databases, file systems, user interfaces, or other
    /// external systems. This pool provides centralized access to all output channels.
    /// </summary>
    public sealed class ExternalTransmitters : IPool<IExternalTransmitter>
    {
        /// <summary>
        /// Gets or sets the collection of active external transmitter instances.
        /// Contains all external transmitters that are available for outputting
        /// processed workflow results to external destinations.
        /// </summary>
        public List<IExternalTransmitter> Pool { get; set; }

        /// <summary>
        /// Holds the singleton instance of the ExternalTransmitters pool.
        /// </summary>
        private static ExternalTransmitters _instance;

        /// <summary>
        /// Initializes a new instance of the ExternalTransmitters class.
        /// Private constructor to enforce singleton pattern and prevent external instantiation.
        /// Creates an empty list of external transmitters ready for registration.
        /// </summary>
        private ExternalTransmitters()
        {
            Pool = new List<IExternalTransmitter>();
        }

        /// <summary>
        /// Gets the singleton instance of the ExternalTransmitters pool.
        /// Creates the instance on first access if it doesn't exist (lazy initialization).
        /// Thread-safe singleton implementation for managing external transmitters.
        /// </summary>
        public static ExternalTransmitters Instance
        {
            get
            {
                return _instance ?? (_instance = new ExternalTransmitters());
            }
        }
    }
}