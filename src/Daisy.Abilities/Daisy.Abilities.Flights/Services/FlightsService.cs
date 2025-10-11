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
            // AviationStack API - search for flights by arrival city (IATA code or city name)
            var response = await _httpClient.GetAsync($"flights?access_key={_settings.ApiKey}&arr_iata={city}&limit=3");
            if (!response.IsSuccessStatusCode)
            {
                return string.Empty;
            }

            var content = await response.Content.ReadAsStringAsync();
            
            // For demonstration, return the raw JSON response
            // In production, you might want to parse and format this nicely
            return content;
        }
    }
}