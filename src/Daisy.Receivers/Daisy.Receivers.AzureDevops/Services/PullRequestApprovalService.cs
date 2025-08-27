using Daisy.Receivers.AzureDevopsReceiver.Models;
using Daisy.Resources.Models;
using Daisy.Resources.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;
using System.Threading.Tasks;

namespace Daisy.Receivers.AzureDevopsReceiver.Services
{
    public class PullRequestApprovalService : IPullRequestApprovalService
    {
        private readonly GitSettings _settings;
        private HttpClient _client = null!;

        public PullRequestApprovalService(ApplicationSettings settings)
        {
            _settings = settings.GetApiSettings<GitSettings>("Git");
        }

        public void Initialize(IServiceProvider serviceProvider)
        {
            _client = serviceProvider.GetService<IHttpClientFactory>()!.CreateClient("DefaultClient");
            var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes($":{_settings.PersonalAccessToken}"));
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);
            _client.BaseAddress = new Uri($"https://dev.azure.com/{_settings.Organization}/");
        }

        public async Task<string> GetPullRequestStatusAsync(UserStory story, int pullRequestId)
        {
            var response = await _client.GetAsync($"{_settings.Project}/_apis/git/repositories/{story.Repository}/pullRequests/{pullRequestId}?api-version=7.0");
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Http error in pull request approval.");
            }

            using var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
            return json.RootElement.GetProperty("status").GetString();
        }
    }
}
