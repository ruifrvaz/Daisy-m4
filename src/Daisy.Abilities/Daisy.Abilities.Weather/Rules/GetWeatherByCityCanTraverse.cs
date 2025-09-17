using Daisy.Abilities.Weather.Paths;
using Daisy.Resources.Attributes;
using Daisy.Resources.Extensions;
using Daisy.Resources.Interfaces;
using Daisy.Resources.Models;
using Daisy.Resources.Signals;

namespace Daisy.Abilities.Weather.Rules
{
    [TraverseRule(PathType = typeof(GetWeatherByCityPath))]
    public class GetWeatherByCityCanTraverse : ITraverseRule
    {
        private readonly ApplicationSettings _settings;

        public GetWeatherByCityCanTraverse(ApplicationSettings settings)
        {
            _settings = settings;
        }

        public bool RuleApplies(Impulse impulse)
        {
            return !string.IsNullOrEmpty(impulse.GetChainByKey("weather"));
        }
    }
}
