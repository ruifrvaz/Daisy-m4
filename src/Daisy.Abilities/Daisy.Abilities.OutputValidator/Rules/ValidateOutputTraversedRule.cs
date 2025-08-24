using System;
using Daisy.Resources.Signals;
using Daisy.Resources.Interfaces;
using Daisy.Resources.Attributes;
using Daisy.Abilities.OutputValidator.Paths;
using Daisy.Resources.Models;

namespace Daisy.Abilities.OutputValidator.Rules
{
    [TraversedRule(PathType = typeof(OutputValidatorPath))]
    public class ValidateOutputTraversedRule : ITraverseRule
    {


        private ApplicationSettings _settings;

        public ValidateOutputTraversedRule(ApplicationSettings settings)
        {
            _settings = settings;
        }

        public bool RuleApplies(Impulse impulse)
        {
            return !string.IsNullOrEmpty(impulse.Output);
        }
    }
}