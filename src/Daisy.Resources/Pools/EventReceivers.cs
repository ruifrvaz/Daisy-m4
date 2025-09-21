using Daisy.Resources.Interfaces;
using System.Collections.Generic;

namespace Daisy.Resources.Pools
{
    /// <summary>
    /// Singleton pool that manages all event receiver instances in the Daisy workflow engine.
    /// Event receivers handle asynchronous event-based input patterns, enabling workflows to respond
    /// to various event sources such as timer triggers, message queue notifications, custom application
    /// events, or external system events. This pool provides centralized access to event-driven input sources.
    /// </summary>
    public sealed class EventReceivers : IPool<IEventReceiver>
    {
        /// <summary>
        /// Gets or sets the collection of active event receiver instances.
        /// Contains all event receivers that are available for processing
        /// event-based input and triggering workflow execution.
        /// </summary>
        public List<IEventReceiver> Pool { get; set; }

        /// <summary>
        /// Holds the singleton instance of the EventReceivers pool.
        /// </summary>
        private static EventReceivers _instance;

        /// <summary>
        /// Initializes a new instance of the EventReceivers class.
        /// Private constructor to enforce singleton pattern and prevent external instantiation.
        /// Creates an empty list of event receivers ready for registration.
        /// </summary>
        private EventReceivers()
        {
            Pool = new List<IEventReceiver>();
        }

        /// <summary>
        /// Gets the singleton instance of the EventReceivers pool.
        /// Creates the instance on first access if it doesn't exist (lazy initialization).
        /// Thread-safe singleton implementation for managing event receivers.
        /// </summary>
        public static EventReceivers Instance
        {
            get
            {
                return _instance ?? (_instance = new EventReceivers());
            }
        }
    }
}