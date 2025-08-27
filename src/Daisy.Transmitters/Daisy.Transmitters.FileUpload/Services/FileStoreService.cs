using Daisy.Resources.Models;
using Daisy.Transmitters.VectorStore.Models;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace Daisy.Transmitters.VectorStore.Services
{
    public class FileStoreService : IFileStoreService
    {
        private HttpClient _httpClient;
        private AssistantSettings _openAISettings;

        public FileStoreService(ApplicationSettings settings)
        {
            _openAISettings = settings.GetApiSettings<AssistantSettings>("OpenAI");
        }

        public void Initialize(IServiceProvider serviceProvider)
        {
            _httpClient = serviceProvider.GetService<IHttpClientFactory>().CreateClient("DefaultClient");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _openAISettings.ApiKey);
            _httpClient.DefaultRequestHeaders.Add("OpenAI-Beta", "assistants=v2");
        }


        public async Task<string> UploadFileAsync(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentException("File path cannot be empty", nameof(filePath));
            }

            await using var fileStream = System.IO.File.OpenRead(filePath);

            using var form = new MultipartFormDataContent();
            form.Add(new StringContent("assistants"), "purpose");
            form.Add(new StreamContent(fileStream), "file", System.IO.Path.GetFileName(filePath));

            var response = await _httpClient.PostAsync($"{_openAISettings.ApiBaseUrl}/files", form);
            var jsonResponse = await response.Content.ReadAsStringAsync();

            try
            {
                var responseObject = JsonSerializer.Deserialize<FileResponse>(jsonResponse);
                return responseObject?.id ?? jsonResponse;
            }
            catch
            {
                return jsonResponse;
            }
        }


        public async Task<string> LoadIntoVectorStoreAsync(string vectorStoreId, string fileId)
        {
            if (string.IsNullOrWhiteSpace(vectorStoreId))
            {
                throw new ArgumentException("Assistant id cannot be empty", nameof(vectorStoreId));
            }

            if (string.IsNullOrWhiteSpace(fileId))
            {
                throw new ArgumentException("File id cannot be empty", nameof(fileId));
            }

            var requestBody = new { file_id = fileId };
            var jsonRequest = JsonSerializer.Serialize(requestBody);
            var requestContent = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{_openAISettings.ApiBaseUrl}/vector_stores/{vectorStoreId}/files", requestContent);
            var jsonResponse = await response.Content.ReadAsStringAsync();

            try
            {
                var responseObject = JsonSerializer.Deserialize<AssistantFileResponse>(jsonResponse);
                return responseObject?.id ?? jsonResponse;
            }
            catch
            {
                return jsonResponse;
            }
        }

        public async Task<string> LoadIntoCodeInterpreterAsync(string assistantId, string fileId)
        {
            if (string.IsNullOrWhiteSpace(assistantId))
            {
                throw new ArgumentException("Assistant id cannot be empty", nameof(assistantId));
            }

            if (string.IsNullOrWhiteSpace(fileId))
            {
                throw new ArgumentException("File id cannot be empty", nameof(fileId));
            }

            var updatePayload = new
            {
                tool_resources = new
                {
                    code_interpreter = new
                    {
                        file_ids = new[]
                                {
                                    fileId
                                }
                    }
                },
                tools = new[]
                {
                    new { type = "code_interpreter" }
                }
            };

            var jsonRequest = JsonSerializer.Serialize(updatePayload);
            var requestContent = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{_openAISettings.ApiBaseUrl}/assistants/{assistantId}", requestContent);
            var jsonResponse = await response.Content.ReadAsStringAsync();

            try
            {
                var responseObject = JsonSerializer.Deserialize<AssistantFileResponse>(jsonResponse);
                return responseObject?.id ?? jsonResponse;
            }
            catch
            {
                return jsonResponse;
            }
        }
    }
}