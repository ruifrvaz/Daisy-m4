using Daisy.Abilities.Assistant.Operator.Paths;
using Daisy.Resources.Attributes;
using Daisy.Resources.Interfaces;
using Daisy.Resources.Models;
using Daisy.Resources.Signals;
using System;

namespace Daisy.Abilities.Assistant.Operator.Rules
{
    [TraversedRule(PathType = typeof(OperatorPath))]
    public class OperatorTraversedRule : ITraverseRule
    {
        private ApplicationSettings _settings;

        public OperatorTraversedRule(ApplicationSettings settings)
        {
            _settings = settings;
        }

        public bool RuleApplies(Impulse impulse)
        {
            return impulse.Output.StartsWith("Operator:", StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
