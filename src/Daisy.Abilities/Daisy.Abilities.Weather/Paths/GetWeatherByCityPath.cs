using Daisy.Abilities.Weather.Services;
using Daisy.Abilities.Weather.Services;
using Daisy.Resources.Abstracts;
using Daisy.Resources.Extensions;
using Daisy.Resources.Interfaces;
using Daisy.Resources.Models;
using Daisy.Resources.Services;
using Daisy.Resources.Signals;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Daisy.Abilities.Weather.Paths
{
    public class GetWeatherByCityPath : APath
    {
        private readonly IWeatherService _weatherService;

        public GetWeatherByCityPath(IServiceProvider serviceProvider,
            IEnumerable<ITraverseRule> traverseRules,
            IEnumerable<ITraverseRule> hasBeenTraversedRules,
            string pathName,
            int traverseOrder,
            ApplicationSettings settings)
            : base(serviceProvider, traverseRules, hasBeenTraversedRules, pathName, traverseOrder, settings)
        {
            _weatherService = ServiceContainer.Instance.GetService<IWeatherService>() as IWeatherService;
        }

        public override async Task Traverse(Impulse impulse)
        {
            var cityName = impulse.GetChainByKey("cityName");
            var weather = cityName == null ? null : await _weatherService.GetWeatherAsync(cityName);

            if (string.IsNullOrWhiteSpace(weather))
            {
                impulse.Error = $"Unable to retrieve weather for {cityName}.";
            }
            else
            {
                impulse.Output = weather;
            }

            await Emit(impulse);
        }
    }
}