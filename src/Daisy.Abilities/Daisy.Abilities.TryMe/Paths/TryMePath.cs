using Daisy.Abilities.TryMe.Services;
using Daisy.Resources.Abstracts;
using Daisy.Resources.Extensions;
using Daisy.Resources.Interfaces;
using Daisy.Resources.Models;
using Daisy.Resources.Signals;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Daisy.Abilities.TryMe.Paths
{
    public class TryMePath : APath
    {
        private readonly WeatherService _weatherService;

        public TryMePath(IServiceProvider serviceProvider,
            IEnumerable<ITraverseRule> traverseRules,
            IEnumerable<ITraverseRule> hasBeenTraversedRules,
            string pathName,
            int traverseOrder,
            ApplicationSettings settings)
            : base(serviceProvider, traverseRules, hasBeenTraversedRules, pathName, traverseOrder, settings)
        {
            _weatherService = new WeatherService(new HttpClient());
        }

        public override async Task Traverse(Impulse impulse)
        {
            var cityName = impulse.Input.GetChainByKey("cityName");
            var weather = cityName == null ? null : await _weatherService.GetWeatherAsync(cityName);

            if (string.IsNullOrWhiteSpace(weather))
            {
                impulse.Output = $"Unable to retrieve weather for {cityName}.";
            }
            else
            {
                impulse.Output = weather;
            }

            await Emit(impulse);
        }
    }
}
