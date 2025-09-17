using Daisy.Abilities.Weather.Paths;
using Daisy.Resources.Attributes;
using Daisy.Resources.Extensions;
using Daisy.Resources.Interfaces;
using Daisy.Resources.Models;
using Daisy.Resources.Pools;
using Daisy.Resources.Signals;
using System.Linq;

namespace Daisy.Abilities.Terminate.Rules
{
    [TraversedRule(PathType = typeof(GetWeatherByCityPath))]
    public class GetWeatherByCityTraversed : ITraverseRule
    {

        private ApplicationSettings _settings;

        public GetWeatherByCityTraversed(ApplicationSettings settings)
        {
            _settings = settings;
        }

        public bool RuleApplies(Impulse impulse)
        {
            return !string.IsNullOrWhiteSpace(impulse.GetChainByKey("weather", ImpulseExtensions.ImpulseField.Output));
        }
    }
}
