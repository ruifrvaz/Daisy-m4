using Daisy.Resources.Abstracts;
using Daisy.Resources.Extensions;
using Daisy.Resources.Signals;

namespace Daisy.Receivers.AzureDevopsReceiver
{
    public class WorkItemStatusReceiverLoopback : ALoopBackReceiver
    {
        public override bool CanReceive(Impulse impulse)
        {
            return impulse.Input.StartsWith("WorkitemStatus:", StringComparison.InvariantCultureIgnoreCase);
        }

        public async override Task ReceiveLoopBack(Impulse impulse)
        {
            if (CanReceive(impulse))
            {
                var status = impulse.Input.GetChainByKey("WorkitemStatus");
                impulse.Output = impulse.Output.AddPrefix($"WorkitemStatus: {status} > ");

                Console.WriteLine($"17. Work item status: {status}.");
                await base.ReceiveLoopBack(impulse);
            }
        }
    }
}
