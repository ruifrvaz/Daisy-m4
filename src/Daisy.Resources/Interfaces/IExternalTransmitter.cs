using System;
using Daisy.Resources.Signals;

namespace Daisy.Resources.Interfaces
{
    /// <summary>
    /// Defines the contract for external transmitter components that output processed results to external systems.
    /// External transmitters represent the final stage of workflow processing, sending Impulse outputs
    /// to external destinations such as APIs, databases, file systems, or user interfaces.
    /// They serve as the exit points from the workflow orchestration engine.
    /// </summary>
    public interface IExternalTransmitter
    {
        /// <summary>
        /// Determines whether this transmitter can handle the given impulse for external output.
        /// Evaluates transmission rules and conditions to decide if the impulse should be processed.
        /// </summary>
        /// <param name="impulse">The impulse to evaluate for transmission capability</param>
        /// <returns>True if the transmitter can handle this impulse, false otherwise</returns>
        bool CanTransmit(Impulse impulse);

        /// <summary>
        /// Transmits the impulse output to an external destination.
        /// This method performs the final stage of workflow processing by sending
        /// the processed results to external systems, users, or storage locations.
        /// </summary>
        /// <param name="impulse">The impulse containing the output data to transmit</param>
        void Transmit(Impulse impulse);
    }
}