using Daisy.Receivers.PipelineRunner.Models;
using Daisy.Receivers.PipelineRunner.Services;
using Daisy.Resources.Abstracts;
using Daisy.Resources.Extensions;
using Daisy.Resources.Services;
using Daisy.Resources.Signals;

namespace Daisy.Receivers.PipelineRunner
{
    public class PipelineRunnerReceiverLoopback : ALoopBackReceiver
    {
        private readonly IPipelineStatusService _service;

        public PipelineRunnerReceiverLoopback()
        {
            _service = ServiceContainer.Instance.GetService<IPipelineStatusService>() as IPipelineStatusService;
        }

        public override bool CanReceive(Impulse impulse)
        {
            return impulse.Input.StartsWith("PipelineRunId:", StringComparison.InvariantCultureIgnoreCase);
        }

        public async override Task ReceiveLoopBack(Impulse impulse)
        {
            if (!CanReceive(impulse))
            {
                return;
            }

            var runId = impulse.Input.GetChainByKey("PipelineRunId");
            var pipelineId = impulse.Input.GetChainByKey("PipelineId");
            var storyJson = impulse.Output.GetChainByKey("StoryParsed");
            string status = string.Empty;
            if (!string.IsNullOrWhiteSpace(runId) && !string.IsNullOrWhiteSpace(storyJson))
            {
                var story = storyJson.GetData<UserStory>();
                

                if (story != null)
                {
                    const int totalAttempts = 30;
                    for (int i = 0; i < totalAttempts; i++)
                    {
                        status = await _service.GetPipelineRunStatusAsync(pipelineId, runId);
                        Console.WriteLine($"    Pipeline run status: {status}. Attempt {i + 1}/{totalAttempts}");

                        if (status.Equals("succeeded", StringComparison.InvariantCultureIgnoreCase) ||
                            status.Equals("failed", StringComparison.InvariantCultureIgnoreCase) ||
                            status.Equals("canceled", StringComparison.InvariantCultureIgnoreCase))
                        {
                            break;
                        }
                        await Task.Delay(TimeSpan.FromSeconds(15));
                    }
                }

                if (string.IsNullOrWhiteSpace(status))
                {
                    impulse.Error = "Pipeline run timed out.";
                }
                else
                {
                    impulse.Input = impulse.Input.AddPrefix($"PipelineStatus: {status} > ");
                }
            }

            Console.WriteLine($"14. Run finished: {status}.");
            await base.ReceiveLoopBack(impulse);
        }
    }
}