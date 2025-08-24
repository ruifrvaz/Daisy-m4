using Daisy.Abilities.Assistant.Operator.Models;
using Daisy.Resources.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Daisy.Abilities.Assistant.Operator.Services
{
    public class OperatorAssistantService : IAssistantService
    {
        private HttpClient _httpClient;
        private AssistantSettings _settings;

        public OperatorAssistantService(ApplicationSettings settings)
        {
            _settings = settings.GetApiSettings<AssistantSettings>("OpenAI");
        }

        public void Initialize(IServiceProvider serviceProvider)
        {
            _httpClient = serviceProvider.GetService<IHttpClientFactory>()!.CreateClient("DefaultClient");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _settings.ApiKey);
            _httpClient.DefaultRequestHeaders.Add("OpenAI-Beta", "assistants=v2");
        }

        public async Task<string> CreateThreadAsync()
        {
            var requestContent = new StringContent(string.Empty, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"{_settings.ApiBaseUrl}/threads", requestContent);
            var jsonResponse = await response.Content.ReadAsStringAsync();
            var responseObject = JsonSerializer.Deserialize<ThreadResponse>(jsonResponse);
            return responseObject!.id;
        }

        public async Task<string> AddMessageToThreadAsync(string message, string role, string threadId)
        {
            var requestBody = new { role, content = message };
            var jsonRequest = JsonSerializer.Serialize(requestBody);
            var requestContent = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"{_settings.ApiBaseUrl}/threads/{threadId}/messages", requestContent);
            var jsonResponse = await response.Content.ReadAsStringAsync();
            var responseObject = JsonSerializer.Deserialize<ThreadResponse>(jsonResponse);
            return responseObject!.id;
        }

        public async Task<string> RunAssistantOnThreadAsync(string assistantId, string threadId)
        {
            var requestBody = new { assistant_id = assistantId };
            var jsonRequest = JsonSerializer.Serialize(requestBody);
            var requestContent = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"{_settings.ApiBaseUrl}/threads/{threadId}/runs", requestContent);
            var jsonResponse = await response.Content.ReadAsStringAsync();
            var responseObject = JsonSerializer.Deserialize<ThreadResponse>(jsonResponse);
            return responseObject!.id;
        }

        public async Task<string> CheckRunStatusAsync(string threadId, string runId)
        {
            var response = await _httpClient.GetAsync($"{_settings.ApiBaseUrl}/threads/{threadId}/runs/{runId}");
            if (!response.IsSuccessStatusCode)
            {
                return string.Empty;
            }
            var jsonResponse = await response.Content.ReadFromJsonAsync<RunResponse>();
            return jsonResponse!.status;
        }

        public async Task<ThreadConversation> DisplayMessagesAsync(string threadId)
        {
            var response = await _httpClient.GetAsync($"{_settings.ApiBaseUrl}/threads/{threadId}/messages");
            var jsonResponse = await response.Content.ReadAsStringAsync();
            var responseObject = JsonSerializer.Deserialize<ThreadConversation>(jsonResponse);
            return responseObject!;
        }

        public async Task<RunDetails> GetRunDetailsAsync(string threadId, string runId)
        {
            var url = $"{_settings.ApiBaseUrl}/threads/{threadId}/runs/{runId}";
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<RunDetails>(content);
        }

        public string DetermineFunction(string assistantMessage)
        {
            if (string.IsNullOrWhiteSpace(assistantMessage))
            {
                return string.Empty;
            }

            try
            {
                var doc = JsonDocument.Parse(assistantMessage);
                if (doc.RootElement.TryGetProperty("function", out var fn))
                {
                    return fn.GetString() ?? string.Empty;
                }
            }
            catch
            {
                // ignore if not json
            }

            var match = Regex.Match(assistantMessage, @"Function:(\w+)", RegexOptions.IgnoreCase);
            if (match.Success)
            {
                return match.Groups[1].Value;
            }

            return string.Empty;
        }

        public async Task SubmitToolOutputsAsync(string threadId, string runId, ToolOutput[] toolOutputs)
        {
            var url = $"{_settings.ApiBaseUrl}/threads/{threadId}/runs/{runId}/submit_tool_outputs";

            var payload = new
            {
                tool_outputs = toolOutputs
            };

            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json")
            };

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
        }
    }
}
