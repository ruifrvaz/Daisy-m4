using Daisy.Resources.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Daisy.Transmitters.AzureDevOps.Models;
using Daisy.Resources.Signals;
using Daisy.Resources.Extensions;

namespace Daisy.Transmitters.AzureDevOps.Services
{
    public class AzureDevOpsService : IAzureDevOpsService
    {
        private HttpClient _client = null!;
        private readonly AzureDevOpsSettings _settings;

        public AzureDevOpsService(ApplicationSettings settings)
        {
            _settings = settings.GetApiSettings<AzureDevOpsSettings>("AzureDevOps");
        }

        public void Initialize(IServiceProvider serviceProvider)
        {
            _client = serviceProvider.GetService<IHttpClientFactory>()!.CreateClient("DefaultClient");
            var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes($":{_settings.PersonalAccessToken}"));
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);
            _client.BaseAddress = new Uri($"https://dev.azure.com/{_settings.Organization}/");
        }

        public async Task<int> CreateWorkItemAsync(UserStory story)
        {
            var url = $"{_settings.Project}/_apis/wit/workitems/$User%20Story?api-version=7.0";

            var description = $"{story.Description}\n\n" +
                               $"Repository: {story.Repository}\n" +
                               $"Branch: {story.Branch}\n" +
                               $"Application: {story.Application}\n" +
                               $"Pipeline: {story.Pipeline}";

            var patchDocument = new[]
            {
                new { op = "add", path = "/fields/System.Title", value = story.Title },
                new { op = "add", path = "/fields/System.Description", value = description },
                new { op = "add", path = "/fields/Microsoft.VSTS.Common.AcceptanceCriteria", value = string.Join("\n", story.AcceptanceCriteria) }
            };

            var content = new StringContent(JsonSerializer.Serialize(patchDocument), Encoding.UTF8, "application/json-patch+json");
            var response = await _client.PostAsync(url, content);
            response.EnsureSuccessStatusCode();

            using var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
            return json.RootElement.GetProperty("id").GetInt32();
        }

        public async Task<string> CloseWorkItemAsync(int workItemId, Impulse impulse)
        {
            var url = $"{_settings.Project}/_apis/wit/workitems/{workItemId}?api-version=7.0";

            var pipelineStatus = impulse.Input.GetChainByKey("PipelineStatus");
            var healthStatus = impulse.Output.GetChainByKey("HealthCheck");

            var message = $"Pipeline: {pipelineStatus}; Application Health: {healthStatus}. \n\n Input Metadata: {impulse.Input}.";

            var patchDocument = new[]
            {
                new { op = "add", path = "/fields/System.State", value = "Closed" },
                new { op = "add", path = "/fields/System.History", value = message }
            };

            var content = new StringContent(JsonSerializer.Serialize(patchDocument), Encoding.UTF8, "application/json-patch+json");
            var request = new HttpRequestMessage(new HttpMethod("PATCH"), url)
            {
                Content = content
            };

            var response = await _client.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                return $"error: {await response.Content.ReadAsStringAsync()}";
            }

            return "closed";
        }
    }
}
