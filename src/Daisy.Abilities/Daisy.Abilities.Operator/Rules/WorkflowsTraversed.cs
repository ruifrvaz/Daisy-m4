using Daisy.Resources.Attributes;
using Daisy.Resources.Extensions;
using Daisy.Resources.Interfaces;
using Daisy.Resources.Models;
using Daisy.Resources.Signals;
using System;

namespace Daisy.Abilities.Operator.Rules
{
    [TraversedRule(PathType = typeof(ListWorkflowsPath))]
    public class WorkflowsTraversed : ITraverseRule
    {
        private ApplicationSettings _settings;

        public WorkflowsTraversed(ApplicationSettings settings)
        {
            _settings = settings;
        }

        public bool RuleApplies(Impulse impulse)
        {
            var lastChain = impulse.GetLastChain("WorkflowsPath", ImpulseExtensions.ImpulseField.Output);

            return !string.IsNullOrEmpty(lastChain);
        }
    }
}
