using Daisy.Abilities.Assistant.Operator.Models;
using Daisy.Abilities.Assistant.Operator.Services;
using Daisy.Resources.Abstracts;
using Daisy.Resources.Extensions;
using Daisy.Resources.Interfaces;
using Daisy.Resources.Models;
using Daisy.Resources.Pools;
using Daisy.Resources.Services;
using Daisy.Resources.Signals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Daisy.Abilities.Assistant.Operator.Paths
{
    public class OperatorPath : APath
    {
        private readonly IAssistantService _assistantService;

        public OperatorPath(
            IServiceProvider serviceProvider,
            IEnumerable<ITraverseRule> traverseRules,
            IEnumerable<ITraverseRule> hasBeenTraversedRules,
            string pathName,
            int traverseOrder,
            ApplicationSettings settings)
            : base(serviceProvider, traverseRules, hasBeenTraversedRules, pathName, traverseOrder, settings)
        {
            _assistantService = ServiceContainer.Instance.GetService<IAssistantService>() as IAssistantService;
        }

        public override async Task Traverse(Impulse impulse)
        {
            var input = impulse.Input.GetChainByKey("Operator") ?? impulse.Input;

            var threadId = await _assistantService.CreateThreadAsync();
            await _assistantService.AddMessageToThreadAsync(input, "user", threadId);
            var runId = await _assistantService.RunAssistantOnThreadAsync("asst_jodgVxhpNN0n4m2P1cz2ugVR", threadId);

            var functionName = string.Empty;
            var status = string.Empty;
            do
            {
                status = await _assistantService.CheckRunStatusAsync(threadId, runId);

                await Task.Delay(1000);

                if (status.Equals("requires_action", StringComparison.OrdinalIgnoreCase))
                {
                    // Get tool call info
                    var runInfo = await _assistantService.GetRunDetailsAsync(threadId, runId);
                    var toolCall = runInfo.required_action.submit_tool_outputs.tool_calls[0];

                    functionName = toolCall.function.name;
                    var argsJson = toolCall.function.arguments;
                    var toolCallId = toolCall.id;

                    // Execute the function locally
                    var args = JsonSerializer.Deserialize<Dictionary<string, string>>(argsJson);

                    var eventImpulse = new Impulse
                    {
                        Input = $"Filepath: {args["image_path"]}"
                    };
                    EventReceivers.Instance.Pool.ForEach(receiver => receiver.RaiseEvent(eventImpulse));

                    // Submit result back to OpenAI
                    await _assistantService.SubmitToolOutputsAsync(threadId, runId, new[]
                    {
                        new ToolOutput
                        {
                            tool_call_id = toolCallId,
                            output = "Workflow has started. No further action required."
                        }
                    });
                }
            } while (!status.Equals("completed", StringComparison.OrdinalIgnoreCase));

            var threadMessages = await _assistantService.DisplayMessagesAsync(threadId);
            var message = threadMessages.data
                                     .SelectMany(d => d.content)
                                     .Select(c => c.text)
                                     .First();

            impulse.Output = impulse.Output.AddPrefix($"Operator: {message.value}");

            await Emit(impulse);
        }
    }
}