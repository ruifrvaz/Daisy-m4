using Daisy.Resources.Interfaces;
using Daisy.Resources.Pools;
using Daisy.Resources.Services;
using Daisy.Resources.Signals;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Daisy.Resources.Abstracts
{
    /// <summary>
    /// Abstract base class for internal loop-back receiver components in the Daisy workflow engine.
    /// Loop-back receivers handle Impulses that originate from within the workflow system itself,
    /// typically sent by transmitters that need to trigger additional processing cycles, workflow recursion,
    /// or conditional re-processing patterns.
    /// 
    /// Key characteristics:
    /// - Process impulses from internal workflow sources (not external input)
    /// - Enable complex workflow patterns like iteration and recursive processing
    /// - Automatically route impulses through appropriate transmitters or paths
    /// - Should be registered in pools rather than called directly
    /// </summary>
    public abstract class ALoopBackReceiver : ILoopBackReceiver
    {
        /// <summary>
        /// Gets the path finder service for determining workflow traversal.
        /// Initialized in the constructor.
        /// </summary>
        protected IPathFinder PathFinder { get; set; }

        /// <summary>
        /// Initializes a new instance of the ALoopBackReceiver class.
        /// Sets up the path finder service from the ServiceContainer.
        /// </summary>
        protected ALoopBackReceiver()
        {
            PathFinder = ServiceContainer.Instance.GetService<IPathFinder>() as IPathFinder;
        }

        /// <summary>
        /// Gets or sets the service provider for dependency injection.
        /// Enables access to registered services such as IPathFinder within receiver implementations.
        /// </summary>
        public IServiceProvider ServiceProvider { get; set; }
        /// <summary>
        /// Determines whether this receiver can process the given impulse.
        /// Must be implemented by derived classes to define specific reception criteria.
        /// Evaluates internal rules to decide if the impulse should be handled by this receiver.
        /// </summary>
        /// <param name="impulse">The impulse to evaluate for reception capability</param>
        /// <returns>True if the receiver can handle this impulse, false otherwise</returns>
        public abstract bool CanReceive(Impulse impulse);

        /// <summary>
        /// Asynchronously receives and processes an impulse from within the workflow system.
        /// This method implements the core loop-back processing logic by:
        /// 1. Finding the next path to traverse for the impulse
        /// 2. If a path exists, enqueueing and traversing it
        /// 3. If no path but loop-back transmitters can handle it, sending to loop-back transmitters
        /// 4. Otherwise, sending to external transmitters for final output
        /// 
        /// This routing logic enables complex workflow patterns including iteration and recursion.
        /// </summary>
        /// <param name="impulse">The impulse to receive and process</param>
        /// <returns>A task representing the asynchronous loop-back reception operation</returns>
        public async virtual Task ReceiveLoopBack(Impulse impulse)
        {
            var nextPath = PathFinder.FindNextPathToTraverse(impulse);
            if (nextPath != null)
            {
                impulse.TraversedPaths.Enqueue(nextPath);
                await impulse.TraversedPaths.Last().Traverse(impulse);
            }
            else if (TransmitLoopBack(impulse))
            {
                Parallel.ForEach(LoopBackTransmitters.Instance.Pool, p => p.TransmitLoopBack(impulse));
            }
            else
            {
                Parallel.ForEach(ExternalTransmitters.Instance.Pool, transmitter => transmitter.Transmit(impulse));
            }
        }

        /// <summary>
        /// Determines whether the impulse should be transmitted via loop-back transmitters.
        /// Checks if any loop-back transmitters in the pool can handle the given impulse.
        /// </summary>
        /// <param name="impulse">The impulse to evaluate for loop-back transmission</param>
        /// <returns>True if any loop-back transmitter can handle the impulse, false otherwise</returns>
        private bool TransmitLoopBack(Impulse impulse)
        {
            return LoopBackTransmitters.Instance.Pool.Where(tr => tr.CanTransmit(impulse)).Any();
        }
    }
}