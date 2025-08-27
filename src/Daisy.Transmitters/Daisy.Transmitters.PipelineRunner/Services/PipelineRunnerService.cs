using Daisy.Resources.Models;
using Daisy.Transmitters.PipelineRunner.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;
using System.Threading.Tasks;

namespace Daisy.Transmitters.PipelineRunner.Services
{
    public class PipelineRunnerService : IPipelineRunnerService
    {
        private HttpClient _client = null!;
        private readonly PipelineSettings _settings;

        public PipelineRunnerService(ApplicationSettings settings)
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

        public async Task<string> RunPipelineAsync(UserStory story)
        {
            if (string.IsNullOrWhiteSpace(story.Pipeline))
            {
                var existingId = await GetPipelineIdByNameAsync(story.Repository);
                if (!string.IsNullOrWhiteSpace(existingId))
                {
                    story.Pipeline = existingId;
                }
            }

            if(string.IsNullOrWhiteSpace(story.Pipeline)) {

                var repoId = await GetRepositoryIdAsync(story.Repository);
                if (string.IsNullOrWhiteSpace(repoId))
                {
                    return $"error: repository {story.Repository} not found.";
                }

                var newPipeline = new
                {
                    name = story.Repository,
                    configuration = new
                    {
                        type = "yaml",
                        path = "azure-pipelines.yml",
                        repository = new { id = repoId, type = "azureReposGit", name = story.Repository }
                    }
                };
                var createContent = new StringContent(JsonSerializer.Serialize(newPipeline), Encoding.UTF8, "application/json");
                var createUrl = $"{_settings.Project}/_apis/pipelines?api-version=7.0";
                var createResponse = await _client.PostAsync(createUrl, createContent);
                var createBody = await createResponse.Content.ReadAsStringAsync();
                if (!createResponse.IsSuccessStatusCode)
                {
                    return $"error: {createBody}";
                }

                using var createJson = JsonDocument.Parse(createBody);
                story.Pipeline = createJson.RootElement.GetProperty("id").GetRawText();
                System.Console.WriteLine($"    Created new pipeine: {story.Pipeline}.");
            }

            var run = new { resources = new { repositories = new { self = new { refName = $"refs/heads/{story.Branch}" } } } };
            var content = new StringContent(JsonSerializer.Serialize(run), Encoding.UTF8, "application/json");
            var url = $"{_settings.Project}/_apis/pipelines/{story.Pipeline}/runs?api-version=7.0";

            var response = await _client.PostAsync(url, content);

            var body = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                return $"error: {body}";
            }

            using var json = JsonDocument.Parse(body);
            return json.RootElement.GetProperty("id").GetRawText();
        }

        private async Task<string> GetRepositoryIdAsync(string repositoryName)
        {
            if (string.IsNullOrWhiteSpace(repositoryName))
            {
                return string.Empty;
            }

            var repoUrl = $"{_settings.Project}/_apis/git/repositories/{repositoryName}?api-version=7.0";
            var repoResponse = await _client.GetAsync(repoUrl);
            var repoBody = await repoResponse.Content.ReadAsStringAsync();
            if (!repoResponse.IsSuccessStatusCode)
            {
                return string.Empty;
            }

            using var repoJson = JsonDocument.Parse(repoBody);
            return repoJson.RootElement.GetProperty("id").GetString() ?? string.Empty;
        }

        private async Task<string> GetPipelineIdByNameAsync(string pipelineName)
        {
            if (string.IsNullOrWhiteSpace(pipelineName))
            {
                return string.Empty;
            }

            var listUrl = $"{_settings.Project}/_apis/pipelines?api-version=7.0";
            var listResponse = await _client.GetAsync(listUrl);
            var listBody = await listResponse.Content.ReadAsStringAsync();
            if (!listResponse.IsSuccessStatusCode)
            {
                return string.Empty;
            }

            using var listJson = JsonDocument.Parse(listBody);
            foreach (var p in listJson.RootElement.GetProperty("value").EnumerateArray())
            {
                if (p.GetProperty("name").GetString() == pipelineName)
                {
                    return p.GetProperty("id").GetRawText();
                }
            }

            return string.Empty;
        }
    }
}
