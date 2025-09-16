using Daisy.Resources.Attributes;
using Daisy.Resources.Interfaces;
using Daisy.Resources.Models;
using Daisy.Resources.Signals;
using System;

namespace Daisy.Abilities.Start.Rules
{
    [TraversedRule(PathType = typeof(WorkflowsPath))]
    public class WorkflowsTraversed : ITraverseRule
    {
        private ApplicationSettings _settings;

        public WorkflowsTraversed(ApplicationSettings settings)
        {
            _settings = settings;
        }

        public bool RuleApplies(Impulse impulse)
        {
            return impulse.Output.StartsWith("WorkflowsPath:", StringComparison.InvariantCultureIgnoreCase);
        }
    }
}