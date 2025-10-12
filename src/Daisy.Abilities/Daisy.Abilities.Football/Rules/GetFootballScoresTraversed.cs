using Daisy.Abilities.Football.Paths;
using Daisy.Resources.Attributes;
using Daisy.Resources.Extensions;
using Daisy.Resources.Interfaces;
using Daisy.Resources.Models;
using Daisy.Resources.Signals;

namespace Daisy.Abilities.Football.Rules
{
    /// <summary>
    /// Rule to determine if GetFootballScoresPath has been traversed.
    /// Returns true when output chain contains "footballScores: {scores}".
    /// </summary>
    [TraversedRule(PathType = typeof(GetFootballScoresPath))]
    public class GetFootballScoresTraversed : ITraverseRule
    {
        private readonly ApplicationSettings _settings;

        public GetFootballScoresTraversed(ApplicationSettings settings)
        {
            _settings = settings;
        }

        public bool RuleApplies(Impulse impulse)
        {
            return !string.IsNullOrWhiteSpace(impulse.GetChainByKey("footballScores", ImpulseExtensions.ImpulseField.Output));
        }
    }
}
