using Daisy.Resources.Interfaces;
using System.Collections.Generic;

namespace Daisy.Resources.Pools
{
    /// <summary>
    /// Singleton pool that manages all external receiver instances in the Daisy workflow engine.
    /// External receivers handle input from outside the workflow system such as user input,
    /// API calls, webhooks, scheduled events, or file system changes. This pool provides
    /// centralized registration and access to all external input sources.
    /// </summary>
    public sealed class ExternalReceivers : IPool<IExternalReceiver>
    {
        /// <summary>
        /// Gets or sets the collection of active external receiver instances.
        /// Contains all external receivers that are available for processing
        /// input from external sources and triggering workflow execution.
        /// </summary>
        public List<IExternalReceiver> Pool { get; set; }

        /// <summary>
        /// Holds the singleton instance of the ExternalReceivers pool.
        /// </summary>
        private static ExternalReceivers _instance;

        /// <summary>
        /// Initializes a new instance of the ExternalReceivers class.
        /// Private constructor to enforce singleton pattern and prevent external instantiation.
        /// Creates an empty list of external receivers ready for registration.
        /// </summary>
        private ExternalReceivers()
        {
            Pool = new List<IExternalReceiver>();
        }

        /// <summary>
        /// Gets the singleton instance of the ExternalReceivers pool.
        /// Creates the instance on first access if it doesn't exist (lazy initialization).
        /// Thread-safe singleton implementation for managing external receivers.
        /// </summary>
        public static ExternalReceivers Instance
        {
            get
            {
                return _instance ?? (_instance = new ExternalReceivers());
            }
        }
    }
}