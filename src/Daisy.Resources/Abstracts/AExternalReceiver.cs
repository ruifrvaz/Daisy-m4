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
    /// Abstract base class for external receiver components that handle input from outside the workflow engine.
    /// External receivers respond to external stimuli such as user input, API calls, webhooks, scheduled events,
    /// or file system changes. They serve as the primary entry points for triggering workflow processing
    /// by creating and initializing Impulse objects from external data sources.
    /// 
    /// Key characteristics:
    /// - Continuously monitor for external input in an async loop
    /// - Automatically route created impulses through the path finding system
    /// - Include error handling and recovery mechanisms with exception limits
    /// - Support graceful start/stop lifecycle management
    /// </summary>
    public abstract class AExternalReceiver : IExternalReceiver
    {
        /// <summary>
        /// Gets or sets the service provider for dependency injection.
        /// Enables access to registered services such as IPathFinder within receiver implementations.
        /// </summary>
        public IServiceProvider ServiceProvider { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this receiver is currently active and monitoring for input.
        /// Used to control the receiver lifecycle and processing loop.
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Gets the collection of core namespaces where this receiver should be active.
        /// Determines which workflow cores will load and execute this receiver instance.
        /// Must be implemented by derived classes to specify core targeting.
        /// </summary>
        public abstract IEnumerable<string> RunOnCores { get; }

        /// <summary>
        /// Synchronously receives input and creates an Impulse object.
        /// Must be implemented by derived classes to handle specific input sources.
        /// Used for scenarios where synchronous processing is required.
        /// </summary>
        /// <returns>The initialized Impulse with input data</returns>
        public abstract Impulse Receive();

        /// <summary>
        /// Asynchronously receives input and creates an Impulse object.
        /// Must be implemented by derived classes to handle specific input sources.
        /// This is the primary method used in the receiver's processing loop.
        /// </summary>
        /// <returns>A task containing the initialized Impulse with input data</returns>
        public abstract Task<Impulse> ReceiveAsync();

        /// <summary>
        /// Starts the external receiver with continuous monitoring for input.
        /// This method:
        /// 1. Activates the receiver (sets IsActive = true)
        /// 2. Enters a continuous loop to receive and process impulses
        /// 3. Automatically routes impulses through the PathFinder system
        /// 4. Handles exceptions with a limit of 3 consecutive exceptions before stopping
        /// 5. Continues until stopped or exception limit reached
        /// </summary>
        /// <param name="token">The cancellation token to monitor for cancellation requests</param>
        /// <returns>A task representing the asynchronous receiver operation</returns>
        public virtual async Task Start(CancellationToken token)
        {
            IsActive = true;
            var nrExceptions = 0;
            do
            {
                try
                {
                    var impulse = await ReceiveAsync();
                    var pathFinder = ServiceProvider.GetRequiredService<IPathFinder>();
                    var nextPath = pathFinder.FindNextPathToTraverse(impulse);
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
                    System.Console.WriteLine($"{this}, {ex}");
                    nrExceptions++;
                }
            } while (IsActive && nrExceptions < 3);
        }

        /// <summary>
        /// Stops the external receiver and ceases input monitoring.
        /// Sets IsActive to false, which will cause the processing loop to exit
        /// at the next iteration, ensuring graceful shutdown.
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
