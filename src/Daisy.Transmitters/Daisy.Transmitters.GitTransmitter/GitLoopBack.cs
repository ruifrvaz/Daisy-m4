using Daisy.Resources.Extensions;
using Daisy.Resources.Interfaces;
using Daisy.Resources.Pools;
using Daisy.Resources.Services;
using Daisy.Resources.Signals;
using Daisy.Transmitters.GitTransmitter.Models;
using Daisy.Transmitters.GitTransmitter.Services;
using System.Text.Json;

namespace Daisy.Transmitters.GitTransmitter
{
    public class GitLoopBack : ILoopBackTransmitter
    {
        private readonly IGitPullRequestService _service;

        public GitLoopBack()
        {
            _service = ServiceContainer.Instance.GetService<IGitPullRequestService>() as IGitPullRequestService;
        }

        public bool CanTransmit(Impulse impulse)
        {
            return impulse.Output.StartsWith("FilesParsed:") && impulse.Input.StartsWith($"FileStored:");
        }

        public void TransmitLoopBack(Impulse impulse)
        {
            if (!CanTransmit(impulse))
            {
                return;
            }

            var storyJson = impulse.Output.GetChainByKey("StoryParsed");
            var repoZip = impulse.Output.GetChainByKey("GitRepoFileLocation");
            var filesJson = impulse.Output.GetChainByKey("FilesParsed");

            if (string.IsNullOrWhiteSpace(storyJson) || string.IsNullOrWhiteSpace(repoZip) || string.IsNullOrWhiteSpace(filesJson))
            {
                impulse.Error = $"{nameof(GitLoopBack)} missing data.";
                Parallel.ForEach(ExternalTransmitters.Instance.Pool, t => t.Transmit(impulse));
                return;
            }

            var story = storyJson.GetData<UserStory>();
            var files = JsonSerializer.Deserialize<List<CoderFileMetadata>>(filesJson) ?? new();

            var prId = _service.CommitFilesAndCreatePullRequestAsync(story, files, repoZip).GetAwaiter().GetResult();

            impulse.Input = impulse.Input.AddPrefix($"PullRequestId: {prId} > ");

            System.Console.WriteLine($"11. Pull request created {prId}.");
            LoopBackReceivers.Instance.DispatchAsync(impulse);
        }
    }
}