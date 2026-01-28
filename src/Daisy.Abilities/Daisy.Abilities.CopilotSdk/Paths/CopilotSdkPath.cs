using Daisy.Abilities.CopilotSdk.Services;
using Daisy.Resources.Abstracts;
using Daisy.Resources.Extensions;
using Daisy.Resources.Interfaces;
using Daisy.Resources.Models;
using Daisy.Resources.Services;
using Daisy.Resources.Signals;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Daisy.Abilities.CopilotSdk.Paths
{
    /// <summary>
    /// Path that processes prompts using the GitHub Copilot SDK.
    /// Extracts prompts from the impulse chain and sends them to Copilot for AI-powered responses.
    /// </summary>
    public class CopilotSdkPath : APath
    {
        private readonly ICopilotSdkService _copilotSdkService;

        public CopilotSdkPath(
            IServiceProvider serviceProvider,
            IEnumerable<ITraverseRule> traverseRules,
            IEnumerable<ITraverseRule> hasBeenTraversedRules,
            string pathName,
            int traverseOrder,
            ApplicationSettings settings)
            : base(serviceProvider, traverseRules, hasBeenTraversedRules, pathName, traverseOrder, settings)
        {
            _copilotSdkService = ServiceContainer.Instance.GetService<ICopilotSdkService>() as ICopilotSdkService;
        }

        public override async Task Traverse(Impulse impulse)
        {
            // Extract the prompt from the impulse chain
            var prompt = impulse.GetChainByKey("copilot");

            if (_copilotSdkService == null || !_copilotSdkService.IsAvailable())
            {
                impulse.Error = "Copilot SDK service is not available. Please ensure Copilot CLI is installed and authenticated.";
                await Emit(impulse);
                return;
            }

            if (string.IsNullOrWhiteSpace(prompt))
            {
                impulse.Error = "No prompt provided for Copilot SDK.";
                await Emit(impulse);
                return;
            }

            // Send the prompt to Copilot SDK and get the response
            var response = await _copilotSdkService.SendPromptAsync(prompt);

            if (string.IsNullOrWhiteSpace(response))
            {
                impulse.Error = $"Unable to get a response from Copilot SDK for prompt: {prompt}";
            }
            else
            {
                // Add the response to the output chain
                impulse.AddChain($"Copilot: {response}", ImpulseExtensions.ImpulseField.Output);
            }

            await Emit(impulse);
        }
    }
}
