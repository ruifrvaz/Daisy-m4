using Daisy.Resources.Signals;

namespace Daisy.Resources.Interfaces
{
    /// <summary>
    /// Defines the contract for internal loop-back transmitter components in the Daisy workflow engine.
    /// Loop-back transmitters enable workflow recursion and iteration by sending processed Impulses
    /// back into the workflow system for additional processing cycles. This creates complex workflow
    /// patterns where results can trigger new processing chains or conditional re-execution.
    /// </summary>
    public interface ILoopBackTransmitter
    {
        /// <summary>
        /// Determines whether this transmitter can handle the given impulse for loop-back transmission.
        /// Evaluates internal rules and conditions to decide if the impulse should be sent back
        /// into the workflow system for additional processing.
        /// </summary>
        /// <param name="impulse">The impulse to evaluate for loop-back transmission capability</param>
        /// <returns>True if the transmitter can handle this impulse, false otherwise</returns>
        bool CanTransmit(Impulse impulse);

        /// <summary>
        /// Transmits the impulse back into the workflow system for additional processing.
        /// This method enables workflow recursion by sending the impulse to loop-back receivers,
        /// allowing for iterative processing, conditional re-execution, or workflow chaining.
        /// </summary>
        /// <param name="impulse">The impulse to transmit back into the workflow system</param>
        void TransmitLoopBack(Impulse impulse);
    }
}
