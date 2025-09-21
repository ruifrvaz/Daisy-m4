using Daisy.Resources.Signals;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Daisy.Resources.Interfaces
{
    /// <summary>
    /// Defines the base contract for all receiver components in the Daisy workflow engine.
    /// Receivers are entry points that ingest external input and initialize Impulse objects
    /// to start workflow processing. They act as triggers that can respond to various
    /// external stimuli such as user input, webhooks, or scheduled events.
    /// </summary>
    public interface IReceiver : ITask
    {
        /// <summary>
        /// Gets the collection of core namespaces where this receiver should be active.
        /// Determines which workflow cores will load and execute this receiver instance.
        /// </summary>
        IEnumerable<string> RunOnCores { get; }

        /// <summary>
        /// Asynchronously receives input and creates an Impulse object.
        /// This is the primary method for processing external input in an async context.
        /// </summary>
        /// <returns>A task containing the initialized Impulse with input data</returns>
        Task<Impulse> ReceiveAsync();

        /// <summary>
        /// Synchronously receives input and creates an Impulse object.
        /// Used for scenarios where synchronous processing is required.
        /// </summary>
        /// <returns>The initialized Impulse with input data</returns>
        Impulse Receive();
    }
}
