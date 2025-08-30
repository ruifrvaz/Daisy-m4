using Daisy.Resources.Extensions;
using Daisy.Resources.Interfaces;
using Daisy.Resources.Pools;
using Daisy.Resources.Services;
using Daisy.Resources.Signals;
using Daisy.Transmitters.PipelineRunner.Models;
using Daisy.Transmitters.PipelineRunner.Services;
using System.Net.NetworkInformation;

namespace Daisy.Transmitters.PipelineRunner
{
    public class PipelineRunnerLoopBack : ILoopBackTransmitter
    {
        private readonly IPipelineRunnerService _service;

        public PipelineRunnerLoopBack()
        {
            _service = ServiceContainer.Instance.GetService<IPipelineRunnerService>() as IPipelineRunnerService;
        }

        public bool CanTransmit(Impulse impulse)
        {
            return impulse.Input.StartsWith("PullRequestStatus:");
        }

        public void TransmitLoopBack(Impulse impulse)
        {
            if (!CanTransmit(impulse))
            {
                return;
            }

            var storyJson = impulse.Output.GetChainByKey("StoryParsed");
            if (string.IsNullOrWhiteSpace(storyJson))
            {
                impulse.Error = $"{nameof(PipelineRunnerLoopBack)} missing story.";
                Parallel.ForEach(ExternalTransmitters.Instance.Pool, p => p.Transmit(impulse));
                return;
            }

            var story = storyJson.GetData<UserStory>();
            var runId = _service.RunPipelineAsync(story).GetAwaiter().GetResult();

            impulse.Input = impulse.Input.AddPrefix($"PipelineRunId: {runId} > PipelineId: {story.Pipeline} >");

            System.Console.WriteLine($"13. Started pipeline run {runId}.");
            LoopBackReceivers.Instance.DispatchAsync(impulse);
        }
    }
}
