using Daisy.Resources.Attributes;
using Daisy.Resources.Interfaces;
using Daisy.Resources.Models;
using Daisy.Resources.Pools;
using Daisy.Resources.Signals;
using System.Linq;

namespace Daisy.Abilites.Terminate.Rules
{
    [TraversedRule(PathType = typeof(TerminatePath))]
    public class TerminateTraversed : ITraverseRule
    {

        private ApplicationSettings _settings;

        public TerminateTraversed(ApplicationSettings settings)
        {
            _settings = settings;
        }

        public bool RuleApplies(Impulse impulse)
        {
            return Cores.Instance.Pool.All(core => !core.IsActive);
        }
    }
}
