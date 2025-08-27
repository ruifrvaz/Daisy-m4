using Daisy.Abilities.GitAssistant.Models;
using Daisy.Abilities.GitAssistant.Services;
using Daisy.Resources.Abstracts;
using Daisy.Resources.Interfaces;
using Daisy.Resources.Models;
using Daisy.Resources.Extensions;
using Daisy.Resources.Services;
using Daisy.Resources.Signals;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Daisy.Abilities.GitAssistant.Paths
{
    public class GitRepositoryFetcherPath : APath
    {
        private readonly IGitService _gitService;
        private readonly GitSettings _settings;

        public GitRepositoryFetcherPath(
            IServiceProvider serviceProvider,
            IEnumerable<ITraverseRule> traverseRules,
            IEnumerable<ITraverseRule> hasBeenTraversedRules,
            string pathName,
            int traverseOrder,
            ApplicationSettings settings)
            : base(serviceProvider, traverseRules, hasBeenTraversedRules, pathName, traverseOrder, settings)
        {
            _gitService = ServiceContainer.Instance.GetService<IGitService>() as IGitService;
            _settings = settings.GetApiSettings<GitSettings>("Git");
        }

        public override async Task Traverse(Impulse impulse)
        {
            var story = impulse.Output.GetData<UserStory>();
            if (story == null || string.IsNullOrWhiteSpace(story.Repository) || string.IsNullOrWhiteSpace(story.Branch))
            {
                impulse.Error = $"Error at {nameof(GitRepositoryFetcherPath)}: Story is invalid.";
                await Emit(impulse); // break cycle and exit
            }

            if (!await _gitService.EnsureRepositoryExistsAsync(story.Repository, story.Branch))
            {
                impulse.Error = $"Error at {nameof(GitRepositoryFetcherPath)}: unable to verify repository.";
                await Emit(impulse);
                return;
            }

            var zipPath = await _gitService.FetchLatestAsync($"https://dev.azure.com/{_settings.Organization}/{_settings.Project}/_git/{story.Repository}", story.Repository, story.Branch);

            impulse.Output = impulse.Output.AddPrefix($"GitRepoFileLocation: {zipPath} > ");

            System.Console.WriteLine($"6. Fetched repository {story.Repository}.");
            await Emit(impulse);
        }
    }
}
