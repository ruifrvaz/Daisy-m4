using Daisy.Resources.Attributes;
using Daisy.Resources.Interfaces;
using Daisy.Resources.Models;
using Daisy.Resources.Signals;

namespace Daisy.Abilities.Operator.Rules
{
    [TraverseRule(PathType = typeof(ListWorkflowsPath))]
    public class WorkflowsCanTraverse : ITraverseRule
    {
        private ApplicationSettings _settings;

        public WorkflowsCanTraverse(ApplicationSettings settings)
        {
            _settings = settings;
        }

        public bool RuleApplies(Impulse impulse)
        {
            return impulse.Input.Equals("start", System.StringComparison.InvariantCultureIgnoreCase);
        }
    }
}