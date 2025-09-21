using Daisy.Resources.Signals;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Daisy.Resources.Interfaces
{
    /// <summary>
    /// Defines the contract for external receiver components that handle input from outside the workflow engine.
    /// External receivers respond to external stimuli such as user input, API calls, webhooks, or scheduled events,
    /// creating Impulse objects to trigger workflow processing. They inherit from IReceiver and extend it
    /// with external-specific capabilities.
    /// </summary>
    public interface IExternalReceiver : IReceiver
    {
    }
}