using Daisy.Abilities.HealthCheck.Paths;
using Daisy.Resources.Attributes;
using Daisy.Resources.Interfaces;
using Daisy.Resources.Models;
using Daisy.Resources.Signals;
using System;

namespace Daisy.Abilities.HealthCheck.Rules
{
    [TraversedRule(PathType = typeof(HealthCheckPath))]
    public class HealthCheckTraversedRule : ITraverseRule
    {
        private readonly ApplicationSettings _settings;
        public HealthCheckTraversedRule(ApplicationSettings settings)
        {
            _settings = settings;
        }

        public bool RuleApplies(Impulse impulse)
        {
            return impulse.Output.Contains("HealthCheck:", StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
