using Daisy.Resources.Extensions;
using Daisy.Resources.Interfaces;
using Daisy.Resources.Pools;
using Daisy.Resources.Services;
using Daisy.Resources.Signals;
using Daisy.Transmitters.AzureDevOps.Services;
using System;

namespace Daisy.Transmitters.AzureDevOps
{
    public class AzureDevOpsCloseWorkItemLoopBack : ILoopBackTransmitter
    {
        private readonly IAzureDevOpsService _service;

        public AzureDevOpsCloseWorkItemLoopBack()
        {
            _service = ServiceContainer.Instance.GetService<IAzureDevOpsService>() as IAzureDevOpsService;
        }

        public bool CanTransmit(Impulse impulse)
        {
            return impulse.Input.Contains("WorkitemId:", StringComparison.InvariantCultureIgnoreCase)
                && impulse.Input.StartsWith("PipelineStatus:", StringComparison.InvariantCultureIgnoreCase);
        }

        public void TransmitLoopBack(Impulse impulse)
        {
            if (!CanTransmit(impulse))
            {
                return;
            }

            var idString = impulse.Input.GetChainByKey("WorkitemId");

            if (int.TryParse(idString, out int workItemId))
            {
                var result = _service.CloseWorkItemAsync(workItemId, impulse).GetAwaiter().GetResult();
                impulse.Input = impulse.Input.AddPrefix($"WorkitemStatus: {result} > ");
            }

            System.Console.WriteLine($"16. Updated and closed work item.");
            LoopBackReceivers.Instance.DispatchAsync(impulse);
        }
    }
}