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
    public class CodeParserPath : APath
    {
        private readonly ICodeParserService _assistantCoder;

        public CodeParserPath(
            IServiceProvider serviceProvider,
            IEnumerable<ITraverseRule> traverseRules,
            IEnumerable<ITraverseRule> hasBeenTraversedRules,
            string pathName,
            int traverseOrder,
            ApplicationSettings settings)
            : base(serviceProvider, traverseRules, hasBeenTraversedRules, pathName, traverseOrder, settings)
        {
            _assistantCoder = ServiceContainer.Instance.GetService<ICodeParserService>() as ICodeParserService;
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

            var serializedFiles = _assistantCoder.ParseCoderFiles(impulse.Output);

            if (string.IsNullOrWhiteSpace(serializedFiles))
            {
                impulse.Error = $"{nameof(CoderPath)} Failed to serialize files.";
                await Emit(impulse);

                return;
            }

            impulse.Output = impulse.Output.AddPrefix($"FilesParsed: {serializedFiles} >");

            System.Console.WriteLine("10. Parsed generated code into files.");
            await Emit(impulse);
        }
    }
}