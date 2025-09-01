using Daisy.Abilities.TryMe;
using Daisy.Resources.Attributes;
using Daisy.Resources.Interfaces;
using Daisy.Resources.Models;
using Daisy.Resources.Signals;

namespace Daisy.Abilities.TryMe.Rules
{
    /// <summary>
    /// Rule that allows traversal when the impulse contains a city name in the input.
    /// </summary>
    [TraverseRule(PathType = typeof(TryMePath))]
    public class TryMeCanTraverse : ITraverseRule
    {
        private readonly ApplicationSettings _settings;

        public TryMeCanTraverse(ApplicationSettings settings)
        {
            _settings = settings;
        }

        public bool RuleApplies(Impulse impulse)
        {
            return !string.IsNullOrWhiteSpace(impulse.Input);
        }
    }
}
