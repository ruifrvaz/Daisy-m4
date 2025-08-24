using Daisy.Abilities.OutputValidator.Paths;
using Daisy.Resources.Attributes;
using Daisy.Resources.Interfaces;
using Daisy.Resources.Models;
using Daisy.Resources.Signals;

namespace Daisy.Abilities.OutputValidator.Rules
{
    [TraverseRule(PathType = typeof(OutputValidatorPath))]
    public class OutputValidatorCanTraverse : ITraverseRule
    {
        private ApplicationSettings _settings;

        public OutputValidatorCanTraverse(ApplicationSettings settings)
        {
            _settings = settings;
        }

        public bool RuleApplies(Impulse impulse)
        {
            return string.IsNullOrEmpty(impulse.Output);
        }
    }
}