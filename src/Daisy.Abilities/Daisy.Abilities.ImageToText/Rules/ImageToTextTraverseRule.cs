using Daisy.Resources.Attributes;
using Daisy.Resources.Interfaces;
using Daisy.Resources.Models;
using Daisy.Resources.Signals;
using System;

namespace Daisy.Abilities.ImageToText.Rules
{
    [TraverseRule(PathType = typeof(Paths.ImageToTextPath))]
    public class ImageToTextTraverseRule : ITraverseRule
    {
        private readonly ApplicationSettings _settings;

        public ImageToTextTraverseRule(ApplicationSettings settings)
        {
            _settings = settings;
        }

        public bool RuleApplies(Impulse impulse)
        {
            return impulse.Input.StartsWith("FilePath:", StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
