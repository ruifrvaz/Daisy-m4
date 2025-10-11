using Daisy.Abilities.Flights.Models;
using Daisy.Abilities.Flights.Services;
using Daisy.Resources.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Daisy.Abilities.Flights.Services
{
    public class FlightsService : IFlightsService
    {
        private readonly FlightsSettings _settings;
        private HttpClient? _httpClient;

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
            // Check if API key is configured
            if (string.IsNullOrWhiteSpace(_settings.ApiKey))
            {
                return GetMockFlightsData(city);
            }

            try
            {
                // Real API implementation using AviationStack
                // API endpoint: /v1/flights with parameters for destination city
                var queryParams = $"?access_key={_settings.ApiKey}&arr_iata={city}&limit=3";
                var response = await _httpClient!.GetAsync(queryParams);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return ParseFlightData(content, city);
                }
                else
                {
                    // Fall back to mock data if API call fails
                    return GetMockFlightsData(city);
                }
            }
            catch (Exception)
            {
                // Fall back to mock data on any exception
                return GetMockFlightsData(city);
            }
        }

        private static string ParseFlightData(string jsonResponse, string city)
        {
            try
            {
                using var doc = JsonDocument.Parse(jsonResponse);
                var root = doc.RootElement;

                if (!root.TryGetProperty("data", out var dataArray) || dataArray.GetArrayLength() == 0)
                {
                    return $"No flights found for {city}.";
                }

                var flights = new System.Collections.Generic.List<string>();
                var count = 0;

                foreach (var flight in dataArray.EnumerateArray())
                {
                    if (count >= 3) break;

                    var flightNumber = flight.TryGetProperty("flight", out var flightInfo) &&
                                      flightInfo.TryGetProperty("iata", out var iata)
                        ? iata.GetString() ?? "N/A"
                        : "N/A";

                    var departure = flight.TryGetProperty("departure", out var depInfo) &&
                                   depInfo.TryGetProperty("airport", out var depAirport)
                        ? depAirport.GetString() ?? "N/A"
                        : "N/A";

                    var arrival = flight.TryGetProperty("arrival", out var arrInfo) &&
                                 arrInfo.TryGetProperty("airport", out var arrAirport)
                        ? arrAirport.GetString() ?? city
                        : city;

                    var departureTime = flight.TryGetProperty("departure", out var depTimeInfo) &&
                                       depTimeInfo.TryGetProperty("scheduled", out var scheduled)
                        ? scheduled.GetString() ?? "N/A"
                        : "N/A";

                    flights.Add($"Flight {flightNumber}: {departure} to {arrival} - Departure {departureTime}");
                    count++;
                }

                return flights.Count > 0
                    ? string.Join("\n", flights)
                    : $"No flights found for {city}.";
            }
            catch (Exception)
            {
                return GetMockFlightsData(city);
            }
        }

        private static string GetMockFlightsData(string city)
        {
            // Mock flight data for demonstration when API key is not configured
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