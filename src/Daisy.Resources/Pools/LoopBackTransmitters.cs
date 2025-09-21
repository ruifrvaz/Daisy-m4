using System;
using System.Collections.Generic;
using Daisy.Resources.Interfaces;

namespace Daisy.Resources.Pools
{
    /// <summary>
    /// Singleton pool that manages all loop-back transmitter instances in the Daisy workflow engine.
    /// Loop-back transmitters enable workflow recursion and iteration by sending processed impulses
    /// back into the workflow system for additional processing cycles. This pool provides centralized
    /// access to all internal workflow routing mechanisms that create complex processing patterns.
    /// </summary>
    public sealed class LoopBackTransmitters : IPool<ILoopBackTransmitter>
    {
        /// <summary>
        /// Gets or sets the collection of active loop-back transmitter instances.
        /// Contains all loop-back transmitters that are available for sending
        /// impulses back into the workflow system for recursive processing.
        /// </summary>
        public List<ILoopBackTransmitter> Pool { get; set; }

        /// <summary>
        /// Holds the singleton instance of the LoopBackTransmitters pool.
        /// </summary>
        private static LoopBackTransmitters _instance;

        /// <summary>
        /// Initializes a new instance of the LoopBackTransmitters class.
        /// Private constructor to enforce singleton pattern and prevent external instantiation.
        /// Creates an empty list of loop-back transmitters ready for registration.
        /// </summary>
        private LoopBackTransmitters()
        {
            Pool = new List<ILoopBackTransmitter>();
        }

        /// <summary>
        /// Gets the singleton instance of the LoopBackTransmitters pool.
        /// Creates the instance on first access if it doesn't exist (lazy initialization).
        /// Thread-safe singleton implementation for managing loop-back transmitters.
        /// </summary>
        public static LoopBackTransmitters Instance
        {
            get
            {
                return _instance ?? (_instance = new LoopBackTransmitters());
            }
        }
    }
}