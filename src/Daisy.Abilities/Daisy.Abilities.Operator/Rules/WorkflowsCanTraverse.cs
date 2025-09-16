using Daisy.Resources.Attributes;
using Daisy.Resources.Interfaces;
using Daisy.Resources.Models;
using Daisy.Resources.Signals;

namespace Daisy.Abilities.Start.Rules
{
    [TraverseRule(PathType = typeof(WorkflowsPath))]
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