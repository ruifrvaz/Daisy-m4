using Daisy.Abilities.Assistant.Coder.Paths;
using Daisy.Resources.Attributes;
using Daisy.Resources.Interfaces;
using Daisy.Resources.Models;
using Daisy.Resources.Signals;
using System;

namespace Daisy.Abilities.Assistant.Coder.Rules
{
    [TraversedRule(PathType = typeof(CodeParserPath))]
    public class CodeParserTraversedRule : ITraverseRule
    {
        private ApplicationSettings _settings;

        public CodeParserTraversedRule(ApplicationSettings settings)
        {
            _settings = settings;
        }

        public bool RuleApplies(Impulse impulse)
        {
            return impulse.Output.Contains("FilesParsed:", StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
