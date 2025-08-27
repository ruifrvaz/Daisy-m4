using Daisy.Abilities.GitAssistant.Paths;
using Daisy.Resources.Attributes;
using Daisy.Resources.Interfaces;
using Daisy.Resources.Models;
using Daisy.Resources.Signals;
using System;

namespace Daisy.Abilities.GitAssistant.Rules
{
    [TraverseRule(PathType = typeof(GitRepositoryFetcherPath))]
    public class GitRepositoryFetcherCanTraverseRule : ITraverseRule
    {
        private readonly ApplicationSettings _settings;
        public GitRepositoryFetcherCanTraverseRule(ApplicationSettings settings)
        {
            _settings = settings;
        }

        public bool RuleApplies(Impulse impulse)
        {
            return impulse.Input.StartsWith("WorkitemId:", StringComparison.InvariantCultureIgnoreCase) && 
                   impulse.Output.StartsWith("StoryParsed:", StringComparison.InvariantCultureIgnoreCase) && 
                   !impulse.Output.Contains("GitRepoFileLocation:", StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
