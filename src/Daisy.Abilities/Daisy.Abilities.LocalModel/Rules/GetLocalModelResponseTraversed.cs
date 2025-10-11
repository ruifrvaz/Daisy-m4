using Daisy.Abilities.LocalModel.Paths;
using Daisy.Resources.Attributes;
using Daisy.Resources.Extensions;
using Daisy.Resources.Interfaces;
using Daisy.Resources.Models;
using Daisy.Resources.Signals;

namespace Daisy.Abilities.LocalModel.Rules
{
    [TraversedRule(PathType = typeof(GetLocalModelResponsePath))]
    public class GetLocalModelResponseTraversed : ITraverseRule
    {

        private ApplicationSettings _settings;

        public GetLocalModelResponseTraversed(ApplicationSettings settings)
        {
            _settings = settings;
        }

        public bool RuleApplies(Impulse impulse)
        {
            return !string.IsNullOrWhiteSpace(impulse.GetChainByKey("localmodel", ImpulseExtensions.ImpulseField.Output));
        }
    }
}
