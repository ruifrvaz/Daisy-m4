using Daisy.Abilities.Assistant.Coder.Models;
using Daisy.Resources.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Daisy.Abilities.Assistant.Coder.Services
{
    public class AssistantCoderService : IAssistantService
    {
        private HttpClient _httpClient;
        private AssistantSettings _openAISettings;

        public AssistantCoderService(ApplicationSettings settings)
        {
            _openAISettings = settings.GetApiSettings<AssistantSettings>("OpenAI");
        }

        public void Initialize(IServiceProvider serviceProvider)
        {
            _httpClient = serviceProvider.GetService<IHttpClientFactory>().CreateClient("DefaultClient");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _openAISettings.ApiKey);
            _httpClient.DefaultRequestHeaders.Add("OpenAI-Beta", "assistants=v2");
        }

        public async Task<string> CheckRunStatusAsync(string threadId, string runId)
        {
            var response = await _httpClient.GetAsync($"{_openAISettings.ApiBaseUrl}/threads/{threadId}/runs/{runId}");

            if (!response.IsSuccessStatusCode)
            {
                // handle error, maybe throw or return null/empty or log
                Console.WriteLine($"{response.StatusCode}:{response.Content.ReadAsStringAsync()}");
                return string.Empty;
            }

            var jsonResponse = await response.Content.ReadFromJsonAsync<RunResponse>();

            return jsonResponse.status;
        }

        public async Task<string> AddMessageToThreadAsync(string threadMessage, string threadRole, string threadId)
        {
            var requestContent = new StringContent("");
            if (!string.IsNullOrWhiteSpace(threadMessage))
            {
                var requestBody = new
                {
                    role = threadRole,
                    content = threadMessage
                };
                var jsonRequest = JsonSerializer.Serialize(requestBody);
                requestContent = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
            }

            var response = await _httpClient.PostAsync($"{_openAISettings.ApiBaseUrl}/threads/{threadId}/messages", requestContent);
            var jsonResponse = await response.Content.ReadAsStringAsync();

            // Extracting the generated response from the API
            var responseObject = JsonSerializer.Deserialize<ThreadResponse>(jsonResponse);

            return responseObject.id;
        }

        public async Task<string> CreateThreadAsync()
        {
            var requestContent = new StringContent("", Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{_openAISettings.ApiBaseUrl}/threads", requestContent);
            var jsonResponse = await response.Content.ReadAsStringAsync();

            // Extracting the generated response from the API
            var responseObject = JsonSerializer.Deserialize<ThreadResponse>(jsonResponse);

            return responseObject.id;
        }

        public async Task<string> RunAssistantOnThreadAsync(string runInstructions, string assistantId, string threadId)
        {
            var requestBody = new
            {
                assistant_id = assistantId,
            };
            var jsonRequest = JsonSerializer.Serialize(requestBody);
            var requestContent = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{_openAISettings.ApiBaseUrl}/threads/{threadId}/runs", requestContent);
            var jsonResponse = await response.Content.ReadAsStringAsync();

            // Extracting the generated response from the API
            var responseObject = JsonSerializer.Deserialize<ThreadResponse>(jsonResponse);

            return responseObject.id;
        }

        public async Task<ThreadConversation> DisplayMessagesAsync(string threadId)
        {
            var response = await _httpClient.GetAsync($"{_openAISettings.ApiBaseUrl}/threads/{threadId}/messages");
            var jsonResponse = await response.Content.ReadAsStringAsync();

            // Extracting the generated response from the API
            var responseObject = JsonSerializer.Deserialize<ThreadConversation>(jsonResponse);

            return responseObject;
        }
    }
}