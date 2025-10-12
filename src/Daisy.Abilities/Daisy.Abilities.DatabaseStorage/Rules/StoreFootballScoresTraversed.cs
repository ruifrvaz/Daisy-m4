using Daisy.Abilities.DatabaseStorage.Paths;
using Daisy.Resources.Attributes;
using Daisy.Resources.Extensions;
using Daisy.Resources.Interfaces;
using Daisy.Resources.Models;
using Daisy.Resources.Signals;

namespace Daisy.Abilities.DatabaseStorage.Rules
{
    /// <summary>
    /// Rule to determine if StoreFootballScoresPath has been traversed.
    /// Returns true when output chain contains "Scores stored in database".
    /// </summary>
    [TraversedRule(PathType = typeof(StoreFootballScoresPath))]
    public class StoreFootballScoresTraversed : ITraverseRule
    {
        private readonly ApplicationSettings _settings;

        public StoreFootballScoresTraversed(ApplicationSettings settings)
        {
            _settings = settings;
        }

        public bool RuleApplies(Impulse impulse)
        {
            var output = impulse.GetChainByKey("databaseStored", ImpulseExtensions.ImpulseField.Output);
            return !string.IsNullOrWhiteSpace(output);
        }
    }
}
