using Daisy.Resources.Interfaces;
using Daisy.Resources.Services;
using Daisy.Resources.Signals;
using Daisy.Transmitters.Speech.Services;
using Daisy.Resources.Extensions;
using System;

namespace Daisy.Transmitters.Voice
{
    public class VoiceOutput : IExternalTransmitter
    {
        public bool CanTransmit(Impulse impulse)
        {
            return impulse.Output.StartsWith("WorkitemStatus:", StringComparison.InvariantCultureIgnoreCase);
        }

        public void Transmit(Impulse impulse)
        {
            if (!CanTransmit(impulse))
            {
                return;
            }

            var speechService = ServiceContainer.Instance.GetService<ISpeechOutputService>() as ISpeechOutputService;

            if (!string.IsNullOrWhiteSpace(impulse.Error))
            {
                speechService.Speak(impulse.Error);
            }
            else
            {
                var pipelineStatus = impulse.Input.GetChainByKey("PipelineStatus") ?? "unknown";
                var workItemStatus = impulse.Input.GetChainByKey("WorkitemStatus") ?? "unknown";
                var healthStatus = impulse.Output.GetChainByKey("HealthCheck") ?? "unknown";

                var message = $"Workflow complete. Pipeline status {pipelineStatus}. Application is {healthStatus}. Work item is {workItemStatus}. ";

                Console.WriteLine($"18. Workflow complete.");
                speechService.Speak(message);
            }
        }
    }
}