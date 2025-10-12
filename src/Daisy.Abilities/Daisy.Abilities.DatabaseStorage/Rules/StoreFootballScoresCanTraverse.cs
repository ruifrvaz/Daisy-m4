using Daisy.Abilities.DatabaseStorage.Paths;
using Daisy.Resources.Attributes;
using Daisy.Resources.Extensions;
using Daisy.Resources.Interfaces;
using Daisy.Resources.Models;
using Daisy.Resources.Signals;

namespace Daisy.Abilities.DatabaseStorage.Rules
{
    /// <summary>
    /// Rule to determine if StoreFootballScoresPath can traverse.
    /// Returns true when output chain contains "footballScores: {scores}".
    /// </summary>
    [TraverseRule(PathType = typeof(StoreFootballScoresPath))]
    public class StoreFootballScoresCanTraverse : ITraverseRule
    {
        private readonly ApplicationSettings _settings;

        public StoreFootballScoresCanTraverse(ApplicationSettings settings)
        {
            _settings = settings;
        }

        public bool RuleApplies(Impulse impulse)
        {
            return !string.IsNullOrEmpty(impulse.GetChainByKey("footballScores", ImpulseExtensions.ImpulseField.Output));
        }
    }
}
