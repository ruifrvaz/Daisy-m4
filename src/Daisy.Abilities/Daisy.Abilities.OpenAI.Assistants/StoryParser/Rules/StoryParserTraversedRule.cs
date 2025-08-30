using Daisy.Resources.Attributes;
using Daisy.Resources.Interfaces;
using Daisy.Resources.Models;
using Daisy.Resources.Signals;
using System;
using Daisy.Abilities.Assistant.StoryParser.Paths;

namespace Daisy.Abilities.Assistant.StoryParser.Rules
{
    [TraversedRule(PathType = typeof(StoryParserPath))]
    public class StoryParserTraversedRule : ITraverseRule
    {

        private ApplicationSettings _settings;

        public StoryParserTraversedRule(ApplicationSettings settings)
        {
            _settings = settings;
        }

        public bool RuleApplies(Impulse impulse)
        {
            return impulse.Output.StartsWith("StoryParsed:", StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
