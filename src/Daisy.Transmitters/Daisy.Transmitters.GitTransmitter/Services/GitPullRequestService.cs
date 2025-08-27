using Daisy.Resources.Models;
using Daisy.Transmitters.GitTransmitter.Models;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using System.IO.Compression;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Daisy.Transmitters.GitTransmitter.Services
{
    public class GitPullRequestService : IGitPullRequestService
    {
        private readonly GitSettings _settings;
        private HttpClient _client = null!;
        private const string GITEXEPATH = @"C:\Program Files\Git\bin\git.exe";
        public GitPullRequestService(ApplicationSettings settings)
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

        public async Task<int> CommitFilesAndCreatePullRequestAsync(UserStory story, IEnumerable<CoderFileMetadata> files, string repoZipPath)
        {
            var repoDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            ZipFile.ExtractToDirectory(repoZipPath, repoDir);

            var branchName = $"daisy-{Guid.NewGuid():N}".Substring(0, 8);
            await RunGitAsync(repoDir, $"checkout -b {branchName}");

            foreach (var file in files)
            {
                var path = Path.Combine(repoDir, file.Path);
                Directory.CreateDirectory(Path.GetDirectoryName(path)!);
                await File.WriteAllTextAsync(path, file.Content);
            }

            await RunGitAsync(repoDir, "add .");
            await RunGitAsync(repoDir, "commit -m \"Daisy automated changes\"");

            var repoUrl = $"https://dev.azure.com/{_settings.Organization}/{_settings.Project}/_git/{story.Repository}";
            var repoWithCreds = repoUrl;
            try
            {
                if (!string.IsNullOrWhiteSpace(_settings.PersonalAccessToken))
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

            await RunGitAsync(repoDir, $"push {repoWithCreds} {branchName}");

            var pr = new
            {
                sourceRefName = $"refs/heads/{branchName}",
                targetRefName = $"refs/heads/{story.Branch}",
                title = story.Title,
                description = story.Description
            };
            var content = new StringContent(JsonSerializer.Serialize(pr), Encoding.UTF8, "application/json");
            var response = await _client.PostAsync($"{_settings.Project}/_apis/git/repositories/{story.Repository}/pullrequests?api-version=7.0", content);
            response.EnsureSuccessStatusCode();

            using var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
            return json.RootElement.GetProperty("pullRequestId").GetInt32();
        }


        private static async Task RunGitAsync(string workingDir, string args)
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo(GITEXEPATH, args)
                {
                    WorkingDirectory = workingDir,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                }
            };

            process.Start();
            await process.WaitForExitAsync();

            if (process.ExitCode != 0)
            {
                var error = await process.StandardError.ReadToEndAsync();
                throw new InvalidOperationException(error);
            }
        }
    }
}
