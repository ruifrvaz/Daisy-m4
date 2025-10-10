using Daisy.Abilities.Flights.Models;
using Daisy.Abilities.Flights.Services;
using Daisy.Resources.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Daisy.Abilities.Flights.Services
{
    public class FlightsService : IFlightsService
    {
        private readonly FlightsSettings _settings;
        private HttpClient _httpClient;

        public FlightsService(ApplicationSettings settings)
        {
            _settings = settings.GetApiSettings<FlightsSettings>("Flights");
        }

        public void Initialize(IServiceProvider serviceProvider)
        {
            _httpClient = serviceProvider.GetRequiredService<IHttpClientFactory>().CreateClient("DefaultClient");

            _httpClient.BaseAddress = new Uri(_settings.Url);
        }


        public async Task<string> GetFlightsAsync(string city)
        {
            // For demonstration purposes, we'll create a mock response since we don't have a real flights API
            //TODO: In a real implementation, this would call an actual flights API like Amadeus, Skyscanner, etc.
            await Task.Delay(500); // Simulate API call delay

            var mockFlights = new[]
            {
                $"Flight AA101: New York to {city} - Departure 08:00, Arrival 11:30 - $299",
                $"Flight UA205: Chicago to {city} - Departure 14:15, Arrival 17:45 - $245",
                $"Flight DL890: Atlanta to {city} - Departure 19:30, Arrival 22:10 - $189"
            };

            return string.Join("\n", mockFlights);
        }
    }
}