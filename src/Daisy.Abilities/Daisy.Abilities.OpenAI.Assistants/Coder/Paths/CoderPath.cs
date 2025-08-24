using Daisy.Abilities.Assistant.Coder.Services;
using Daisy.Abilities.GitAssistant.Models;
using Daisy.Resources.Abstracts;
using Daisy.Resources.Extensions;
using Daisy.Resources.Interfaces;
using Daisy.Resources.Models;
using Daisy.Resources.Services;
using Daisy.Resources.Signals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Daisy.Abilities.Assistant.Coder.Paths
{
    public class CoderPath : APath
    {
        private readonly IAssistantService _assistantCoder;

        public CoderPath(
            IServiceProvider serviceProvider,
            IEnumerable<ITraverseRule> traverseRules,
            IEnumerable<ITraverseRule> hasBeenTraversedRules,
            string pathName,
            int traverseOrder,
            ApplicationSettings settings)
            : base(serviceProvider, traverseRules, hasBeenTraversedRules, pathName, traverseOrder, settings)
        {
            _assistantCoder = ServiceContainer.Instance.GetService<IAssistantService>() as IAssistantService;
        }

        public override async Task Traverse(Impulse impulse)
        {
            var serializedUserStory = impulse.Output.GetChainByKey("StoryParsed");
            if (string.IsNullOrWhiteSpace(serializedUserStory) || !serializedUserStory.TryGetData(out UserStory _))
            {
                impulse.Error = $"{nameof(CoderPath)} Failed to fetch user story.";
                await Emit(impulse);

                return;
            }

            // create a thread
            var threadId = await _assistantCoder.CreateThreadAsync();

            // write a message to a thread
            var messageId = await _assistantCoder.AddMessageToThreadAsync(serializedUserStory, "user", threadId);

            // run the assistant on the thread
            var assistantRunId = await _assistantCoder.RunAssistantOnThreadAsync(impulse.Input, "asst_VWtn2qYi5gaT0MPCppKeqLDq", threadId);

            // wait for the assistant to complete
            while (!string.Equals(await _assistantCoder.CheckRunStatusAsync(threadId, assistantRunId), "completed", StringComparison.InvariantCultureIgnoreCase))
            {
                Console.WriteLine($"   Generating code...");
                await Task.Delay(TimeSpan.FromSeconds(5));
            }

            // return the list of thread messages
            var threadMessages = await _assistantCoder.DisplayMessagesAsync(threadId);

            var threadData = threadMessages.data
                                           .SelectMany(d => d.content)
                                           .Select(c => c.text)
                                           .Select(c => c.value)
                                           .ToList().First();

            impulse.Output = impulse.Output.AddPrefix($"Coder: {threadData} >");

            System.Console.WriteLine("9. Generated code.");
            await Emit(impulse);
        }
    }
}