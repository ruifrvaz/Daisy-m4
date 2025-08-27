using Daisy.Abilities.GitAssistant.Paths;
using Daisy.Resources.Attributes;
using Daisy.Resources.Interfaces;
using Daisy.Resources.Models;
using Daisy.Resources.Signals;
using System;

namespace Daisy.Abilities.GitAssistant.Rules
{
    [TraversedRule(PathType = typeof(GitRepositoryFetcherPath))]
    public class GitRepositoryFetcherTraversedRule : ITraverseRule
    {
        private readonly ApplicationSettings _settings;
        public GitRepositoryFetcherTraversedRule(ApplicationSettings settings)
        {
            _settings = settings;
        }

        public bool RuleApplies(Impulse impulse)
        {
            return impulse.Output.StartsWith("GitRepoFileLocation:", StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
