using Daisy.Resources.Signals;
using System.Threading.Tasks;

namespace Daisy.Resources.Interfaces
{
    /// <summary>
    /// Defines the contract for internal loop-back receiver components in the Daisy workflow engine.
    /// Loop-back receivers handle Impulses that are sent from within the workflow system itself,
    /// typically from transmitters that need to trigger additional processing cycles or workflow recursion.
    /// These enable complex workflow patterns like iteration and conditional re-processing.
    /// </summary>
    public interface ILoopBackReceiver
    {
        /// <summary>
        /// Determines whether this receiver can process the given impulse.
        /// Evaluates internal rules to decide if the impulse should be handled by this receiver.
        /// </summary>
        /// <param name="impulse">The impulse to evaluate for reception capability</param>
        /// <returns>True if the receiver can handle this impulse, false otherwise</returns>
        bool CanReceive(Impulse impulse);

        /// <summary>
        /// Asynchronously receives and processes an impulse from within the workflow system.
        /// This method handles internal loop-back scenarios where processing results trigger
        /// additional workflow cycles or recursive processing.
        /// </summary>
        /// <param name="impulse">The impulse to receive and process</param>
        /// <returns>A task representing the asynchronous loop-back reception operation</returns>
        Task ReceiveLoopBack(Impulse impulse);
    }
}
