using Daisy.Abilities.Flights.Services;
using Daisy.Resources.Abstracts;
using Daisy.Resources.Extensions;
using Daisy.Resources.Interfaces;
using Daisy.Resources.Models;
using Daisy.Resources.Services;
using Daisy.Resources.Signals;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Daisy.Abilities.Flights.Paths
{
    public class GetFlightsByCityPath : APath
    {
        private readonly IFlightsService _flightsService;

        public GetFlightsByCityPath(IServiceProvider serviceProvider,
            IEnumerable<ITraverseRule> traverseRules,
            IEnumerable<ITraverseRule> hasBeenTraversedRules,
            string pathName,
            int traverseOrder,
            ApplicationSettings settings)
            : base(serviceProvider, traverseRules, hasBeenTraversedRules, pathName, traverseOrder, settings)
        {
            _flightsService = ServiceContainer.Instance.GetService<IFlightsService>() as IFlightsService;
        }

        public override async Task Traverse(Impulse impulse)
        {
            var cityName = impulse.GetChainByKey("flights");
            var flights = cityName == null ? null : await _flightsService.GetFlightsAsync(cityName);

            if (string.IsNullOrWhiteSpace(flights))
            {
                impulse.Error = $"Unable to retrieve flights for {cityName}.";
            }
            else
            {
                impulse.AddChain($"Flights: {flights}", ImpulseExtensions.ImpulseField.Output);
            }

            await Emit(impulse);
        }
    }
}