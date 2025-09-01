using System.Net.Http;
using System.Threading.Tasks;

namespace Daisy.Abilities.TryMe.Services
{
    public class WeatherService
    {
        private readonly HttpClient _httpClient;

        public WeatherService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string?> GetWeatherAsync(string city)
        {
            var response = await _httpClient.GetAsync($"https://wttr.in/{city}?format=3");
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            return await response.Content.ReadAsStringAsync();
        }
    }
}
