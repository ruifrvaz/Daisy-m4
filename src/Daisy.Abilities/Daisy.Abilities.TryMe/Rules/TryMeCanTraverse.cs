using Daisy.Abilities.TryMe.Paths;
using Daisy.Resources.Attributes;
using Daisy.Resources.Extensions;
using Daisy.Resources.Interfaces;
using Daisy.Resources.Models;
using Daisy.Resources.Signals;

namespace Daisy.Abilities.TryMe.Rules
{
    [TraverseRule(PathType = typeof(TryMePath))]
    public class TryMeCanTraverse : ITraverseRule
    {
        private readonly ApplicationSettings _settings;

        public TryMeCanTraverse(ApplicationSettings settings)
        {
            _settings = settings;
        }

        public bool RuleApplies(Impulse impulse)
        {
            return !string.IsNullOrEmpty(impulse.GetChainByKey("cityName"));
        }
    }
}
