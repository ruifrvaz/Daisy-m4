using Daisy.Abilities.HealthCheck.Paths;
using Daisy.Resources.Attributes;
using Daisy.Resources.Interfaces;
using Daisy.Resources.Models;
using Daisy.Resources.Signals;
using System;

namespace Daisy.Abilities.HealthCheck.Rules
{
    [TraverseRule(PathType = typeof(HealthCheckPath))]
    public class HealthCheckCanTraverseRule : ITraverseRule
    {
        private readonly ApplicationSettings _settings;
        public HealthCheckCanTraverseRule(ApplicationSettings settings)
        {
            _settings = settings;
        }

        public bool RuleApplies(Impulse impulse)
        {
            return impulse.Input.Contains("PipelineStatus:", StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
