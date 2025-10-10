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
            var response = await _httpClient.GetAsync($"flights?destination={city}");
            if (!response.IsSuccessStatusCode)
            {
                return string.Empty;
            }

            return await response.Content.ReadAsStringAsync();
        }
    }
}