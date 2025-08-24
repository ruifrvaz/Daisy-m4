using Daisy.Abilities.Assistant.Coder.Paths;
using Daisy.Resources.Attributes;
using Daisy.Resources.Interfaces;
using Daisy.Resources.Models;
using Daisy.Resources.Signals;
using System;

namespace Daisy.Abilities.Assistant.Coder.Rules
{
    [TraverseRule(PathType = typeof(CoderPath))]
    public class CoderCanTraverseRule : ITraverseRule
    {
        private ApplicationSettings _settings;

        public CoderCanTraverseRule(ApplicationSettings settings)
        {
            _settings = settings;
        }

        public bool RuleApplies(Impulse impulse)
        {
            return impulse.Input.StartsWith("FileStored:", StringComparison.InvariantCultureIgnoreCase) &&
                   impulse.Output.Contains("StoryParsed:", StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
