using Daisy.Resources.Interfaces;
using Daisy.Resources.Signals;
using System.Collections.Generic;

namespace Daisy.Resources.Pools
{
    /// <summary>
    /// Singleton pool that manages all loop-back receiver instances in the Daisy workflow engine.
    /// Loop-back receivers handle impulses that originate from within the workflow system,
    /// enabling complex patterns like iteration, recursion, and conditional re-processing.
    /// This pool provides centralized access and dispatch capabilities for internal workflow communication.
    /// </summary>
    public sealed class LoopBackReceivers : IPool<ILoopBackReceiver>
    {
        /// <summary>
        /// Gets or sets the collection of active loop-back receiver instances.
        /// Contains all loop-back receivers that are available for processing
        /// impulses from internal workflow sources.
        /// </summary>
        public List<ILoopBackReceiver> Pool { get; set; }

        /// <summary>
        /// Holds the singleton instance of the LoopBackReceivers pool.
        /// </summary>
        private static LoopBackReceivers _instance;

        /// <summary>
        /// Initializes a new instance of the LoopBackReceivers class.
        /// Private constructor to enforce singleton pattern and prevent external instantiation.
        /// Creates an empty list of loop-back receivers ready for registration.
        /// </summary>
        private LoopBackReceivers()
        {
            Pool = new List<ILoopBackReceiver>();
        }

        /// <summary>
        /// Gets the singleton instance of the LoopBackReceivers pool.
        /// Creates the instance on first access if it doesn't exist (lazy initialization).
        /// Thread-safe singleton implementation for managing loop-back receivers.
        /// </summary>
        public static LoopBackReceivers Instance
        {
            get
            {
                return _instance ?? (_instance = new LoopBackReceivers());
            }
        }

        /// <summary>
        /// Dispatches an impulse to all loop-back receivers in the pool asynchronously.
        /// Iterates through all registered loop-back receivers and calls their ReceiveLoopBack method
        /// to process the impulse. This enables broadcasting of impulses to multiple internal receivers
        /// for parallel or conditional processing.
        /// </summary>
        /// <param name="impulse">The impulse to dispatch to all loop-back receivers</param>
        public void DispatchAsync(Impulse impulse)
        {
            foreach (var receiver in Pool)
            {
                receiver.ReceiveLoopBack(impulse);
            }
        }
    }
}