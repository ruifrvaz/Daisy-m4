using Daisy.Resources.Attributes;
using Daisy.Resources.Interfaces;
using Daisy.Resources.Models;
using Daisy.Resources.Signals;
using System;

namespace Daisy.Abilities.Terminate.Rules
{
    [TraverseRule(PathType = typeof(TerminatePath))]
    public class TerminateCanTraverse : ITraverseRule
    {
        private ApplicationSettings _settings;

        public TerminateCanTraverse(ApplicationSettings settings)
        {
            _settings = settings;
        }

        public bool RuleApplies(Impulse impulse)
        {
            return impulse.Input.StartsWith("bye", StringComparison.InvariantCultureIgnoreCase);
        }
    }
}