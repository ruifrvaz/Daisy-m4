using Daisy.Abilities.Football.Paths;
using Daisy.Resources.Attributes;
using Daisy.Resources.Extensions;
using Daisy.Resources.Interfaces;
using Daisy.Resources.Models;
using Daisy.Resources.Signals;

namespace Daisy.Abilities.Football.Rules
{
    /// <summary>
    /// Rule to determine if GetFootballScoresPath can traverse.
    /// Returns true when input chain contains "football: {clubName}".
    /// </summary>
    [TraverseRule(PathType = typeof(GetFootballScoresPath))]
    public class GetFootballScoresCanTraverse : ITraverseRule
    {
        private readonly ApplicationSettings _settings;

        public GetFootballScoresCanTraverse(ApplicationSettings settings)
        {
            _settings = settings;
        }

        public bool RuleApplies(Impulse impulse)
        {
            return !string.IsNullOrEmpty(impulse.GetChainByKey("football"));
        }
    }
}
