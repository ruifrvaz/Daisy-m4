using Daisy.Resources.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text;
using System.Threading.Tasks;
using Daisy.Abilities.Assistant.StoryParser.Models;

namespace Daisy.Abilities.Assistant.StoryParser.Services
{
    public class AssistantStoryParserService : IAssistantService
    {
        private HttpClient _httpClient;
        private AssistantSettings _settings;

        public AssistantStoryParserService(ApplicationSettings settings)
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
            var requestContent = new StringContent("", Encoding.UTF8, "application/json");
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
    }
}
