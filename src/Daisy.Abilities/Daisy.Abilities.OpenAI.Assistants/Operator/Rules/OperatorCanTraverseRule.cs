using Daisy.Abilities.Assistant.Operator.Paths;
using Daisy.Resources.Attributes;
using Daisy.Resources.Interfaces;
using Daisy.Resources.Models;
using Daisy.Resources.Signals;
using System;

namespace Daisy.Abilities.Assistant.Operator.Rules
{
    [TraverseRule(PathType = typeof(OperatorPath))]
    public class OperatorCanTraverseRule : ITraverseRule
    {
        private ApplicationSettings _settings;

        public OperatorCanTraverseRule(ApplicationSettings settings)
        {
            _settings = settings;
        }

        public bool RuleApplies(Impulse impulse)
        {
            return impulse.Input.StartsWith("Operator:", StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
