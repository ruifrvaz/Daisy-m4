using Daisy.Abilities.Flights.Paths;
using Daisy.Resources.Attributes;
using Daisy.Resources.Extensions;
using Daisy.Resources.Interfaces;
using Daisy.Resources.Models;
using Daisy.Resources.Signals;

namespace Daisy.Abilities.Flights.Rules
{
    [TraverseRule(PathType = typeof(GetFlightsByCityPath))]
    public class GetFlightsByCityCanTraverse : ITraverseRule
    {
        private readonly ApplicationSettings _settings;

        public GetFlightsByCityCanTraverse(ApplicationSettings settings)
        {
            _settings = settings;
        }

        public bool RuleApplies(Impulse impulse)
        {
            return !string.IsNullOrEmpty(impulse.GetChainByKey("flights"));
        }
    }
}