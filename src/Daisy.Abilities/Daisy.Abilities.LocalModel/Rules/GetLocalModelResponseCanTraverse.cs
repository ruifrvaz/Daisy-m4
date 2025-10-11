using Daisy.Abilities.LocalModel.Paths;
using Daisy.Resources.Attributes;
using Daisy.Resources.Extensions;
using Daisy.Resources.Interfaces;
using Daisy.Resources.Models;
using Daisy.Resources.Signals;

namespace Daisy.Abilities.LocalModel.Rules
{
    [TraverseRule(PathType = typeof(GetLocalModelResponsePath))]
    public class GetLocalModelResponseCanTraverse : ITraverseRule
    {
        private readonly ApplicationSettings _settings;

        public GetLocalModelResponseCanTraverse(ApplicationSettings settings)
        {
            _settings = settings;
        }

        public bool RuleApplies(Impulse impulse)
        {
            return !string.IsNullOrEmpty(impulse.GetChainByKey("localmodel"));
        }
    }
}
