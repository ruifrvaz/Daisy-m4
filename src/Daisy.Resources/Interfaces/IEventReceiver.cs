using Daisy.Resources.Signals;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Daisy.Resources.Interfaces
{
    /// <summary>
    /// Defines the contract for event-driven receiver components in the Daisy workflow engine.
    /// Event receivers handle asynchronous event-based input patterns, allowing workflows
    /// to respond to events such as timer triggers, message queue notifications, or custom events.
    /// They extend the base receiver functionality with event publishing capabilities.
    /// </summary>
    public interface IEventReceiver : IReceiver
    {
        /// <summary>
        /// Raises an event with the provided impulse data.
        /// This method enables event-driven workflow patterns by publishing events
        /// that can trigger additional processing or notify other components.
        /// </summary>
        /// <param name="input">The impulse containing the event data to publish</param>
        void RaiseEvent(Impulse input);
    }
}