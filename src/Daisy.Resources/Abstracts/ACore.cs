using Daisy.Resources.Interfaces;
using System.Threading;
using System.Threading.Tasks;
using Daisy.Resources.Pools;
using System.Linq;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Daisy.Resources.Abstracts
{
    /// <summary>
    /// Abstract base class for workflow execution containers in the Daisy orchestration engine.
    /// Cores are the primary execution threads that manage workflow lifecycles and coordinate between
    /// receivers, abilities, and transmitters. They represent the main workflow execution context
    /// where all processing occurs.
    /// 
    /// Key characteristics:
    /// - Cores are the only actual threads that run in the system
    /// - Paths are not threads but channels that modify and conduct impulses to transmitters
    /// - Each core can run in parallel with others and manages its own set of receivers
    /// - Cores communicate through shared pools for inter-workflow coordination
    /// </summary>
    public abstract class ACore : ICore
    {
        /// <summary>
        /// Gets or sets a value indicating whether this core is currently active and running.
        /// Used to track the operational state and control the core lifecycle.
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Gets or sets the collection of receivers that are loaded and managed by this core.
        /// Receivers are filtered based on their RunOnCores configuration to match this core's namespace.
        /// </summary>
        public virtual List<IReceiver> _receivers { get; set; }

        /// <summary>
        /// Starts the core and its associated receivers with the specified cancellation token.
        /// This method:
        /// 1. Activates the core (sets IsActive = true)
        /// 2. Identifies receivers that should run on this core based on namespace matching
        /// 3. Starts all matching receivers as concurrent tasks
        /// 4. Returns a task that completes when all receiver tasks complete
        /// </summary>
        /// <param name="token">The cancellation token to monitor for cancellation requests</param>
        /// <returns>A task that represents the completion of all receiver startup tasks</returns>
        /// <exception cref="InvalidOperationException">Thrown when the core has no namespace</exception>
        public virtual Task Start(CancellationToken token)
        {
            IsActive = true;

            string coreNamespace = GetType().Namespace ?? throw new InvalidOperationException("Core has no namespace.");

            _receivers = ExternalReceivers.Instance.Pool
                .Where(r => r.RunOnCores.Contains(coreNamespace, StringComparer.OrdinalIgnoreCase))
                .Cast<IReceiver>()
                .ToList();

            var receiverTasks = _receivers
                .Select(receiver => Task.Run(() => receiver.Start(token), token))
                .ToList();

            return Task.WhenAll(receiverTasks);
        }

        /// <summary>
        /// Stops the core and all its associated receivers.
        /// This method performs a clean shutdown by:
        /// 1. Stopping all managed receivers
        /// 2. Setting IsActive to false
        /// 3. Ensuring proper resource cleanup
        /// </summary>
        public virtual void Stop()
        {
            if (IsActive)
            {
                _receivers.ForEach(receiver => receiver.Stop());
                IsActive = false;
            }
        }
    }
}