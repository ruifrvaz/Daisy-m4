using Daisy.Resources.Attributes;
using Daisy.Resources.Interfaces;
using Daisy.Resources.Models;
using Daisy.Resources.Signals;
using System;

namespace Daisy.Abilities.ImageToText.Rules
{
    [TraversedRule(PathType = typeof(Paths.ImageToTextPath))]
    public class ImageToTextTraversedRule : ITraverseRule
    {
        private readonly ApplicationSettings _settings;

        public ImageToTextTraversedRule(ApplicationSettings settings)
        {
            _settings = settings;
        }

        public bool RuleApplies(Impulse impulse)
        {
            return impulse.Output.StartsWith("ImageConverted:", StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
