using Daisy.Resources.Extensions;
using Daisy.Resources.Interfaces;
using Daisy.Resources.Pools;
using Daisy.Resources.Signals;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Daisy.Resources.Abstracts
{
    /// <summary>
    /// Abstract base class for event-driven receiver components in the Daisy workflow engine.
    /// Event receivers handle asynchronous event-based input patterns and enable workflows to respond
    /// to various event sources such as timer triggers, message queue notifications, custom application events,
    /// or external system events. They extend receiver functionality with event publishing capabilities.
    /// 
    /// Key characteristics:
    /// - Support both receiving and raising events for bidirectional event communication
    /// - Continuous monitoring with enhanced error handling that preserves error context
    /// - Automatic routing through the PathFinder system like other receivers
    /// - Integration with external transmitters for event-driven output patterns
    /// </summary>
    public abstract class AEventReceiver : IEventReceiver
    {
        /// <summary>
        /// Gets or sets the service provider for dependency injection.
        /// Enables access to registered services such as IPathFinder within receiver implementations.
        /// </summary>
        public IServiceProvider ServiceProvider { get; set; }
        /// <summary>
        /// Gets the collection of core namespaces where this event receiver should be active.
        /// Determines which workflow cores will load and execute this receiver instance.
        /// Must be implemented by derived classes to specify core targeting.
        /// </summary>
        public abstract IEnumerable<string> RunOnCores { get; }

        /// <summary>
        /// Gets or sets a value indicating whether this event receiver is currently active and monitoring for events.
        /// Used to control the receiver lifecycle and event processing loop.
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Raises an event with the provided impulse data.
        /// Must be implemented by derived classes to define specific event publishing behavior.
        /// This method enables event-driven workflow patterns by publishing events that can trigger
        /// additional processing or notify other components in the system.
        /// </summary>
        /// <param name="input">The impulse containing the event data to publish</param>
        public abstract void RaiseEvent(Impulse input);

        /// <summary>
        /// Synchronously receives event input and creates an Impulse object.
        /// Must be implemented by derived classes to handle specific event sources.
        /// Used for scenarios where synchronous event processing is required.
        /// </summary>
        /// <returns>The initialized Impulse with event data</returns>
        public abstract Impulse Receive();

        /// <summary>
        /// Asynchronously receives event input and creates an Impulse object.
        /// Must be implemented by derived classes to handle specific event sources.
        /// This is the primary method used in the event receiver's processing loop.
        /// </summary>
        /// <returns>A task containing the initialized Impulse with event data</returns>
        public abstract Task<Impulse> ReceiveAsync();

        /// <summary>
        /// Starts the event receiver with continuous monitoring for events.
        /// This method:
        /// 1. Activates the receiver (sets IsActive = true)
        /// 2. Enters a continuous loop to receive and process event impulses
        /// 3. Automatically routes impulses through the PathFinder system
        /// 4. Handles exceptions with enhanced error context preservation
        /// 5. Limits consecutive exceptions to 3 before stopping
        /// 6. Continues until stopped or exception limit reached
        /// </summary>
        /// <param name="token">The cancellation token to monitor for cancellation requests</param>
        /// <returns>A task representing the asynchronous event receiver operation</returns>
        public virtual async Task Start(CancellationToken token)
        {
            IsActive = true;
            var nrExceptions = 0;
            var impulse = new Impulse();
            do
            {
                try
                {
                    impulse = await ReceiveAsync();
                    var pathFinder = ServiceProvider?.GetService<IPathFinder>();
                    var nextPath = pathFinder?.FindNextPathToTraverse(impulse);
                    if (nextPath != null)
                    {
                        impulse.TraversedPaths.Enqueue(nextPath);
                        await impulse.TraversedPaths.Last().Traverse(impulse);
                    }
                    else
                    {
                        Parallel.ForEach(ExternalTransmitters.Instance.Pool, transmitter => transmitter.Transmit(impulse));
                    }
                }
                catch (Exception ex)
                {
                    impulse.Error = impulse.Error.AddPrefix($"{this}, {ex}");
                    nrExceptions++;
                }
            } while (IsActive && nrExceptions < 3);
        }

        /// <summary>
        /// Stops the event receiver and ceases event monitoring.
        /// Sets IsActive to false, which will cause the event processing loop to exit
        /// at the next iteration, ensuring graceful shutdown of event handling.
        /// </summary>
        public virtual void Stop()
        {
            if (IsActive)
            {
                IsActive = false;
            }
        }
    }
}
