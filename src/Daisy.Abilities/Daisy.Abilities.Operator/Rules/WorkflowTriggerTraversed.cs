using Daisy.Abilities.Operator.Paths;
using Daisy.Resources.Attributes;
using Daisy.Resources.Extensions;
using Daisy.Resources.Interfaces;
using Daisy.Resources.Models;
using Daisy.Resources.Signals;

namespace Daisy.Abilities.Operator.Rules
{
    [TraversedRule(PathType = typeof(WorkflowTriggerPath))]
    public class WorkflowTriggerTraversed : ITraverseRule
    {
        private readonly ApplicationSettings _settings;

        public WorkflowTriggerTraversed(ApplicationSettings settings)
        {
            _settings = settings;
        }

        public bool RuleApplies(Impulse impulse)
        {
            return !string.IsNullOrWhiteSpace(impulse.GetChainByKey("workflowIdentifier"));
        }
    }
}

