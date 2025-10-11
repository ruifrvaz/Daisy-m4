using Daisy.Abilities.LocalModel.Models;
using Daisy.Resources.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Daisy.Abilities.LocalModel.Services
{
    public class LocalModelService : ILocalModelService
    {
        private readonly LocalModelSettings _settings;
        private HttpClient _httpClient;

        public LocalModelService(ApplicationSettings settings)
        {
            _settings = settings.GetApiSettings<LocalModelSettings>("LocalModel");
        }

        public void Initialize(IServiceProvider serviceProvider)
        {
            _httpClient = serviceProvider.GetRequiredService<IHttpClientFactory>().CreateClient("DefaultClient");

            _httpClient.BaseAddress = new Uri(_settings.Url);
        }

        public async Task<string> GetResponseAsync(string prompt)
        {
            // Check if API URL is configured
            if (string.IsNullOrWhiteSpace(_settings.Url))
            {
                return GetMockResponse(prompt);
            }

            try
            {
                // Real API implementation using a local model API (e.g., Ollama, LocalAI, etc.)
                // Assuming a simple JSON POST request with prompt
                var requestBody = new
                {
                    model = "llama2", // Default model, can be configurable
                    prompt = prompt,
                    stream = false
                };

                var jsonContent = JsonSerializer.Serialize(requestBody);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await _httpClient!.PostAsync("/api/generate", content);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    return ParseModelResponse(responseContent);
                }
                else
                {
                    // Fall back to mock data if API call fails
                    return GetMockResponse(prompt);
                }
            }
            catch (Exception)
            {
                // Fall back to mock data on any error
                return GetMockResponse(prompt);
            }
        }

        private static string ParseModelResponse(string jsonResponse)
        {
            try
            {
                using var doc = JsonDocument.Parse(jsonResponse);
                if (doc.RootElement.TryGetProperty("response", out var responseElement))
                {
                    return responseElement.GetString() ?? "No response from model.";
                }
                return "Unable to parse model response.";
            }
            catch
            {
                return jsonResponse; // Return raw response if parsing fails
            }
        }

        private static string GetMockResponse(string prompt)
        {
            // Mock response for demonstration when API is not configured
            return $"[Mock Response] You asked: \"{prompt}\". This is a simulated response from a local model. To get real responses, configure the LocalModel API URL in appconfig.json.";
        }
    }
}
