using Daisy.Resources.Attributes;
using Daisy.Resources.Interfaces;
using Daisy.Resources.Models;
using Daisy.Resources.Signals;
using System;

namespace Daisy.Abilities.Start.Rules
{
    [TraversedRule(PathType = typeof(StartPath))]
    public class StartTraversed : ITraverseRule
    {
        private ApplicationSettings _settings;

        public StartTraversed(ApplicationSettings settings)
        {
            _settings = settings;
        }

        public bool RuleApplies(Impulse impulse)
        {
            return impulse.Output.StartsWith("----------------------------------------", StringComparison.InvariantCultureIgnoreCase);
        }
    }
}