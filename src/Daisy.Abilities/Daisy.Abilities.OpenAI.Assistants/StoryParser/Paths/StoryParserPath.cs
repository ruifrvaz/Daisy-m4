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
using Daisy.Abilities.Assistant.StoryParser.Services;

namespace Daisy.Abilities.Assistant.StoryParser.Paths
{
    public class StoryParserPath : APath
    {
        private readonly IAssistantService _assistantParser;

        public StoryParserPath(
            IServiceProvider serviceProvider,
            IEnumerable<ITraverseRule> traverseRules,
            IEnumerable<ITraverseRule> hasBeenTraversedRules,
            string pathName,
            int traverseOrder,
            ApplicationSettings settings)
            : base(serviceProvider, traverseRules, hasBeenTraversedRules, pathName, traverseOrder, settings)
        {
            _assistantParser = ServiceContainer.Instance.GetService<IAssistantService>() as IAssistantService;
        }

        public override async Task Traverse(Impulse impulse)
        {
            var input = impulse.Output.GetChainByKey("ImageConverted");

            var threadId = await _assistantParser.CreateThreadAsync();
            var messageId = await _assistantParser.AddMessageToThreadAsync(input, "user", threadId);
            var assistantRunId = await _assistantParser.RunAssistantOnThreadAsync("asst_hQfIG2PC2pDrYizXSq1Avm6u", threadId);

            // wait for the assistant to complete
            while (!string.Equals(await _assistantParser.CheckRunStatusAsync(threadId, assistantRunId), "completed", StringComparison.InvariantCultureIgnoreCase))
            {
                // check every second if the run has completed.
                await Task.Delay(1000);
            }

            // return the list of thread messages
            var threadMessages = await _assistantParser.DisplayMessagesAsync(threadId);
            var threadData = threadMessages.data
                                           .SelectMany(d => d.content)
                                           .Select(c => c.text)
                                           .Select(c => c.value)
                                           .First();

            impulse.Output = impulse.Output.AddPrefix($"StoryParsed: {threadData} > ");

            System.Console.WriteLine("3. Parsed text to story.");
            await Emit(impulse);
        }
    }
}
