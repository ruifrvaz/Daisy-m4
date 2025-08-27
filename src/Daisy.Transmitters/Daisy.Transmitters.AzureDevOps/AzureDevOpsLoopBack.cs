using Daisy.Resources.Extensions;
using Daisy.Resources.Interfaces;
using Daisy.Resources.Pools;
using Daisy.Resources.Services;
using Daisy.Resources.Signals;
using Daisy.Transmitters.AzureDevOps.Models;
using Daisy.Transmitters.AzureDevOps.Services;

namespace Daisy.Transmitters.AzureDevOps
{
    public class AzureDevOpsLoopBack : ILoopBackTransmitter
    {
        private readonly IAzureDevOpsService _service;

        public AzureDevOpsLoopBack()
        {
            _service = ServiceContainer.Instance.GetService<IAzureDevOpsService>() as IAzureDevOpsService;
        }

        public bool CanTransmit(Impulse impulse)
        {
            return impulse.Output.StartsWith($"StoryParsed:");
        }

        public void TransmitLoopBack(Impulse impulse)
        {
            if (!CanTransmit(impulse))
            {
                return;
            }

            var story = impulse.Output.GetData<UserStory>();
            var id = _service.CreateWorkItemAsync(story).GetAwaiter().GetResult();

            impulse.Input = impulse.Input.AddPrefix($"WorkitemId: {id} > ");

            System.Console.WriteLine($"4. Azure DevOps work item created with id {id}.");
            LoopBackReceivers.Instance.DispatchAsync(impulse);
        }
    }
}
