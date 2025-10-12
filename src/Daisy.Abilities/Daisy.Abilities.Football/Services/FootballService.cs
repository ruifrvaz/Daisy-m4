using Daisy.Abilities.Football.Models;
using Daisy.Abilities.Football.Services;
using Daisy.Resources.Interfaces;
using Daisy.Resources.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Daisy.Abilities.Football.Services
{
    /// <summary>
    /// Service for fetching football scores from an external API.
    /// Falls back to mock data if API key is not configured.
    /// </summary>
    public class FootballService : IFootballService
    {
        private readonly FootballSettings _settings;
        private HttpClient? _httpClient;

        public FootballService(ApplicationSettings settings)
        {
            _settings = settings.GetApiSettings<FootballSettings>("Football");
        }

        public void Initialize(IServiceProvider serviceProvider)
        {
            _httpClient = serviceProvider.GetRequiredService<IHttpClientFactory>().CreateClient("DefaultClient");

            _httpClient.BaseAddress = new Uri(_settings.Url);
        }

        /// <summary>
        /// Fetches football scores for the specified club.
        /// Falls back to mock data if API key is not configured.
        /// </summary>
        public async Task<string> GetFootballScoresAsync(string clubName)
        {
            // Check if API key is configured
            if (string.IsNullOrWhiteSpace(_settings.ApiKey))
            {
                return GetMockFootballScores(clubName);
            }

            try
            {
                // Real API implementation using football-data.org API
                // API endpoint: /v4/teams/{teamId}/matches with season parameter
                var queryParams = $"?apiKey={_settings.ApiKey}&team={clubName}&season=2024";
                var response = await _httpClient!.GetAsync(queryParams);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return ParseFootballScores(content, clubName);
                }
                else
                {
                    // Fall back to mock data if API call fails
                    return GetMockFootballScores(clubName);
                }
            }
            catch (Exception)
            {
                // Fall back to mock data on any exception
                return GetMockFootballScores(clubName);
            }
        }

        /// <summary>
        /// Parses football scores from JSON response.
        /// </summary>
        private static string ParseFootballScores(string jsonResponse, string clubName)
        {
            try
            {
                var doc = JsonDocument.Parse(jsonResponse);
                var matches = doc.RootElement.GetProperty("matches");
                
                if (matches.GetArrayLength() > 0)
                {
                    var result = $"Recent matches for {clubName}:\n";
                    var count = 0;
                    foreach (var match in matches.EnumerateArray())
                    {
                        if (count >= 5) break; // Limit to 5 matches
                        
                        var homeTeam = match.GetProperty("homeTeam").GetProperty("name").GetString();
                        var awayTeam = match.GetProperty("awayTeam").GetProperty("name").GetString();
                        var homeScore = match.GetProperty("score").GetProperty("fullTime").GetProperty("home").GetInt32();
                        var awayScore = match.GetProperty("score").GetProperty("fullTime").GetProperty("away").GetInt32();
                        
                        result += $"  {homeTeam} {homeScore} - {awayScore} {awayTeam}\n";
                        count++;
                    }
                    return result;
                }
                
                return $"No matches found for {clubName}.";
            }
            catch (Exception)
            {
                return GetMockFootballScores(clubName);
            }
        }

        /// <summary>
        /// Returns mock football scores when API is not available.
        /// </summary>
        private static string GetMockFootballScores(string clubName)
        {
            return $"Mock scores for {clubName} (Season 2024):\n" +
                   $"  {clubName} 3 - 1 Team A\n" +
                   $"  Team B 2 - 2 {clubName}\n" +
                   $"  {clubName} 1 - 0 Team C\n" +
                   $"  Team D 0 - 4 {clubName}\n" +
                   $"  {clubName} 2 - 3 Team E";
        }
    }
}
