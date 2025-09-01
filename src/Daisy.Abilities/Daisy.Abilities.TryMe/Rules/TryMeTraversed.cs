using Daisy.Abilities.TryMe;
using Daisy.Resources.Attributes;
using Daisy.Resources.Interfaces;
using Daisy.Resources.Models;
using Daisy.Resources.Signals;
using System;

namespace Daisy.Abilities.TryMe.Rules
{
    /// <summary>
    /// Rule that checks if the TryMe path has already produced an output.
    /// </summary>
    [TraversedRule(PathType = typeof(TryMePath))]
    public class TryMeTraversed : ITraverseRule
    {
        private readonly ApplicationSettings _settings;

        public TryMeTraversed(ApplicationSettings settings)
        {
            _settings = settings;
        }

        public bool RuleApplies(Impulse impulse)
        {
            return impulse.Output.StartsWith("TryMePath", StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
