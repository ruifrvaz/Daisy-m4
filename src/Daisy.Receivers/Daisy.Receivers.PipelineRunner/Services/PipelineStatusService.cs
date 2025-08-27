using Daisy.Receivers.PipelineRunner.Models;
using Daisy.Resources.Models;
using Daisy.Resources.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;
using System.Threading.Tasks;

namespace Daisy.Receivers.PipelineRunner.Services
{
    public class PipelineStatusService : IPipelineStatusService
    {
        private readonly PipelineSettings _settings;
        private HttpClient _client = null!;

        public PipelineStatusService(ApplicationSettings settings)
        {
            _settings = settings.GetApiSettings<PipelineSettings>("AzureDevOps");
        }

        public void Initialize(IServiceProvider serviceProvider)
        {
            _client = serviceProvider.GetService<IHttpClientFactory>()!.CreateClient("DefaultClient");
            var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes($":{_settings.PersonalAccessToken}"));
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);
            _client.BaseAddress = new Uri($"https://dev.azure.com/{_settings.Organization}/");
        }

        public async Task<string> GetPipelineRunStatusAsync(string pipelineId, string runId)
        {
            var url = $"{_settings.Project}/_apis/pipelines/{pipelineId}/runs/{runId}?api-version=7.0";
            var response = await _client.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Http error in pipeline run status.");
            }

            using var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
            var state = json.RootElement.GetProperty("state").GetString();
            if (string.Equals(state, "completed", StringComparison.OrdinalIgnoreCase))
            {
                return json.RootElement.GetProperty("result").GetString();
            }
            return state ?? string.Empty;
        }
    }
}
