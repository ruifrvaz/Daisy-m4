using Daisy.Abilities.Operator.Paths;
using Daisy.Resources.Attributes;
using Daisy.Resources.Interfaces;
using Daisy.Resources.Models;
using Daisy.Resources.Signals;

namespace Daisy.Abilities.Operator.Rules
{
    [TraverseRule(PathType = typeof(WorkflowTriggerPath))]
    public class WorkflowTriggerCanTraverse : ITraverseRule
    {
        private readonly ApplicationSettings _settings;

        public WorkflowTriggerCanTraverse(ApplicationSettings settings)
        {
            _settings = settings;
        }

        public bool RuleApplies(Impulse impulse)
        {
            var availableWorkflows = WorkflowTriggerPath.GetAvailableWorkflowIdentifiers();

            return availableWorkflows.Count > 0
                   && WorkflowTriggerPath.TryExtractWorkflowRequest(impulse.Input, availableWorkflows, out _, out _);
        }
    }
}

