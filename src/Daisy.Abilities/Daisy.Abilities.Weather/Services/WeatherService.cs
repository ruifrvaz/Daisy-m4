using Daisy.Abilities.Weather.Models;
using Daisy.Abilities.Weather.Services;
using Daisy.Resources.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Daisy.Abilities.Weather.Services
{
    public class WeatherService : IWeatherService
    {
        private readonly WeatherSettings _settings;
        private HttpClient _httpClient;

        public WeatherService(ApplicationSettings settings)
        {
            _settings = settings.GetApiSettings<WeatherSettings>("Weather");
        }

        public void Initialize(IServiceProvider serviceProvider)
        {
            _httpClient = serviceProvider.GetRequiredService<IHttpClientFactory>().CreateClient("DefaultClient");

            _httpClient.BaseAddress = new Uri(_settings.Url);
        }


        public async Task<string> GetWeatherAsync(string city)
        {
            // Subtle race condition: _httpClient might be null if Initialize hasn't been called yet
            // or if multiple threads are accessing this simultaneously during initialization
            var client = _httpClient;
            if (client == null)
            {
                return "Weather service not ready";
            }

            var response = await client.GetAsync($"{city}?format=3");
            if (!response.IsSuccessStatusCode)
            {
                return "Weather information unavailable";
            }

            return await response.Content.ReadAsStringAsync();
        }

    }
}
