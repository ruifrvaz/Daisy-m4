using Daisy.Abilities.Flights.Paths;
using Daisy.Resources.Attributes;
using Daisy.Resources.Extensions;
using Daisy.Resources.Interfaces;
using Daisy.Resources.Models;
using Daisy.Resources.Pools;
using Daisy.Resources.Signals;
using System.Linq;

namespace Daisy.Abilities.Flights.Rules
{
    [TraversedRule(PathType = typeof(GetFlightsByCityPath))]
    public class GetFlightsByCityTraversed : ITraverseRule
    {

        private ApplicationSettings _settings;

        public GetFlightsByCityTraversed(ApplicationSettings settings)
        {
            _settings = settings;
        }

        public bool RuleApplies(Impulse impulse)
        {
            return !string.IsNullOrWhiteSpace(impulse.GetChainByKey("flights", ImpulseExtensions.ImpulseField.Output));
        }
    }
}
