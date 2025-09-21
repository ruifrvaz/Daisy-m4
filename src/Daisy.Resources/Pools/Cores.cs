using System;
using System.Collections.Generic;
using Daisy.Resources.Interfaces;

namespace Daisy.Resources.Pools
{
    /// <summary>
    /// Singleton pool that manages all core instances in the Daisy workflow orchestration engine.
    /// Cores are the main execution threads that run workflows and coordinate between receivers,
    /// abilities, and transmitters. This pool provides centralized management of all active
    /// workflow execution contexts and enables inter-core communication.
    /// </summary>
    public sealed class Cores : IPool<ICore>
    {
        /// <summary>
        /// Gets or sets the collection of active core instances.
        /// Contains all cores that are available for executing workflows
        /// and managing receiver-ability-transmitter coordination.
        /// </summary>
        public List<ICore> Pool { get; set; }

        /// <summary>
        /// Holds the singleton instance of the Cores pool.
        /// </summary>
        private static Cores _instance;

        /// <summary>
        /// Initializes a new instance of the Cores class.
        /// Private constructor to enforce singleton pattern and prevent external instantiation.
        /// Creates an empty list of cores ready for registration.
        /// </summary>
        private Cores()
        {
            Pool = new List<ICore>();
        }

        /// <summary>
        /// Gets the singleton instance of the Cores pool.
        /// Creates the instance on first access if it doesn't exist (lazy initialization).
        /// Uses null-coalescing assignment operator for concise thread-safe singleton implementation.
        /// </summary>
        public static Cores Instance
        {
            get
            {
                return _instance ??= new Cores();
            }
        }
    }
}