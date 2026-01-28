using Daisy.Abilities.CopilotSdk.Paths;
using Daisy.Resources.Attributes;
using Daisy.Resources.Extensions;
using Daisy.Resources.Interfaces;
using Daisy.Resources.Models;
using Daisy.Resources.Signals;

namespace Daisy.Abilities.CopilotSdk.Rules
{
    /// <summary>
    /// Rule that determines when the CopilotSdkPath can traverse.
    /// The path can traverse when the impulse contains a "copilot:" chain in the input.
    /// </summary>
    [TraverseRule(PathType = typeof(CopilotSdkPath))]
    public class CopilotSdkCanTraverse : ITraverseRule
    {
        private readonly ApplicationSettings _settings;

        public CopilotSdkCanTraverse(ApplicationSettings settings)
        {
            _settings = settings;
        }

        public bool RuleApplies(Impulse impulse)
        {
            // Can traverse if there's a "copilot:" key in the input chain
            return !string.IsNullOrEmpty(impulse.GetChainByKey("copilot"));
        }
    }
}
