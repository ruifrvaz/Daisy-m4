using Daisy.Resources.Attributes;
using Daisy.Resources.Interfaces;
using Daisy.Resources.Models;
using Daisy.Resources.Signals;

namespace Daisy.Abilities.Start.Rules
{
    [TraverseRule(PathType = typeof(WorkflowTriggerPath))]
    public class WorkflowTriggerCanTraverse : ITraverseRule
    {
        private ApplicationSettings _settings;

        public WorkflowTriggerCanTraverse(ApplicationSettings settings)
        {
            _settings = settings;
        }

        public bool RuleApplies(Impulse impulse)
        {
            return impulse.Input.Equals("Weather: Porto", System.StringComparison.InvariantCultureIgnoreCase);
        }
    }
}