using Daisy.Abilities.GitAssistant.Models;
using Daisy.Resources.Models;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection.Metadata;

namespace Daisy.Abilities.GitAssistant.Services
{
    public class GitService : IGitService
    {
        private readonly GitSettings _settings;
        private HttpClient _client = null!;
        private const string GITEXEPATH = @"C:\Program Files\Git\bin\git.exe";

        public GitService(ApplicationSettings settings)
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

        public async Task<bool> EnsureRepositoryExistsAsync(string repository, string branch)
        {
            var repoUrl = $"{_settings.Project}/_apis/git/repositories/{repository}?api-version=7.0";
            var response = await _client.GetAsync(repoUrl);
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                var createUrl = $"{_settings.Project}/_apis/git/repositories?api-version=7.0";
                var body = JsonSerializer.Serialize(new { name = repository });
                var content = new StringContent(body, Encoding.UTF8, "application/json");
                var createResponse = await _client.PostAsync(createUrl, content);
                if (!createResponse.IsSuccessStatusCode)
                {
                    return false;
                }

                var pushUrl = $"{_settings.Project}/_apis/git/repositories/{repository}/pushes?api-version=7.0";
                var pushBody = JsonSerializer.Serialize(new
                {
                    refUpdates = new[]
                    {
                        new { name = $"refs/heads/{branch}", oldObjectId = "0000000000000000000000000000000000000000" }
                    },
                    commits = new[]
                    {
                        new
                        {
                            comment = "Initial README",
                            changes = new[]
                            {
                                new
                                {
                                    changeType = "add",
                                    item = new { path = "/README.md" },
                                    newContent = new { content = $"# {repository}", contentType = "rawtext" }
                                }
                            }
                        }
                    }
                });
                var pushContent = new StringContent(pushBody, Encoding.UTF8, "application/json");
                var pushResponse = await _client.PostAsync(pushUrl, pushContent);
                return pushResponse.IsSuccessStatusCode;
            }
            return response.IsSuccessStatusCode;
        }

        public async Task<string> FetchLatestAsync(string repoUrl, string repository, string branch)
        {
            var tempDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(tempDir);

            var repoWithCreds = repoUrl;
            try
            {
                if (!string.IsNullOrWhiteSpace(_settings.PersonalAccessToken)
                    && (repoUrl.StartsWith("https", StringComparison.InvariantCultureIgnoreCase) || repoUrl.StartsWith("git@")))
                {
                    var uri = new Uri(repoUrl);
                    var token = Uri.EscapeDataString(_settings.PersonalAccessToken);
                    repoWithCreds = $"{uri.Scheme}://pat:{token}@{uri.Host}{uri.PathAndQuery}";
                }
            }
            catch
            {
                repoWithCreds = repoUrl;
            }

            var process = new Process
            {
                StartInfo = new ProcessStartInfo(GITEXEPATH, $"clone --depth 1 --branch {branch} {repoWithCreds} \"{tempDir}\"")
                {
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                }
            };

            process.Start();
            await process.WaitForExitAsync();

            var zipPath = Path.Combine(Path.GetTempPath(), $"{repository}_{branch}" + ".zip");

            if (File.Exists(zipPath))
            {
                File.Delete(zipPath);  //Ensure deletion before zipping to avoid hanging
            }

            ZipFile.CreateFromDirectory(tempDir, zipPath);

            return zipPath;
        }
    }
}
