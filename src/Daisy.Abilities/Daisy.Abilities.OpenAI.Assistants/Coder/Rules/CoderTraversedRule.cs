using Daisy.Abilities.Assistant.Coder.Paths;
using Daisy.Resources.Attributes;
using Daisy.Resources.Interfaces;
using Daisy.Resources.Models;
using Daisy.Resources.Signals;
using System;

namespace Daisy.Abilities.Assistant.Coder.Rules
{
    [TraversedRule(PathType = typeof(CoderPath))]
    public class CoderTraversedRule : ITraverseRule
    {
        private ApplicationSettings _settings;

        public CoderTraversedRule(ApplicationSettings settings)
        {
            _settings = settings;
        }

        public bool RuleApplies(Impulse impulse)
        {
            return impulse.Output.StartsWith("Coder:", StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
