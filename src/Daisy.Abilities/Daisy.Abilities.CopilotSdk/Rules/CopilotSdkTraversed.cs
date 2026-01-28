using Daisy.Abilities.CopilotSdk.Paths;
using Daisy.Resources.Attributes;
using Daisy.Resources.Extensions;
using Daisy.Resources.Interfaces;
using Daisy.Resources.Models;
using Daisy.Resources.Signals;

namespace Daisy.Abilities.CopilotSdk.Rules
{
    /// <summary>
    /// Rule that determines if the CopilotSdkPath has already been traversed.
    /// The path is considered traversed if there's a "Copilot:" chain in the output.
    /// </summary>
    [TraversedRule(PathType = typeof(CopilotSdkPath))]
    public class CopilotSdkTraversed : ITraverseRule
    {
        private readonly ApplicationSettings _settings;

        public CopilotSdkTraversed(ApplicationSettings settings)
        {
            _settings = settings;
        }

        public bool RuleApplies(Impulse impulse)
        {
            // Has been traversed if there's a "Copilot:" key in the output chain
            return !string.IsNullOrWhiteSpace(impulse.GetChainByKey("Copilot", ImpulseExtensions.ImpulseField.Output));
        }
    }
}
