using Daisy.Resources.Attributes;
using Daisy.Resources.Interfaces;
using Daisy.Resources.Models;
using Daisy.Resources.Signals;
using System;
using Daisy.Abilities.Assistant.StoryParser.Paths;

namespace Daisy.Abilities.Assistant.StoryParser.Rules
{
    [TraverseRule(PathType = typeof(StoryParserPath))]
    public class StoryParserCanTraverseRule : ITraverseRule
    {

        private ApplicationSettings _settings;

        public StoryParserCanTraverseRule(ApplicationSettings settings)
        {
            _settings = settings;
        }

        public bool RuleApplies(Impulse impulse)
        {
            return impulse.Input.StartsWith("FilePath:", StringComparison.InvariantCultureIgnoreCase) &&
                   impulse.Output.StartsWith("ImageConverted:", StringComparison.InvariantCultureIgnoreCase) &&
                   !impulse.Output.Contains("StoryParsed:");
        }
    }
}
