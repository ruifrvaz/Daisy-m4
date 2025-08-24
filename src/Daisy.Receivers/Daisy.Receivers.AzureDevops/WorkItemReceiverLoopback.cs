using Daisy.Resources.Abstracts;
using Daisy.Resources.Signals;

namespace Daisy.Receivers.AzureDevopsReceiver
{
    public class WorkItemReceiverLoopback : ALoopBackReceiver
    {
        public override bool CanReceive(Impulse impulse)
        {
            return impulse.Input.StartsWith("WorkitemId:", StringComparison.InvariantCultureIgnoreCase);
        }

        public async override Task ReceiveLoopBack(Impulse impulse)
        {
            if (CanReceive(impulse))
            {
                System.Console.WriteLine("5. Work item received.");
                await base.ReceiveLoopBack(impulse);
            }
        }
    }
}