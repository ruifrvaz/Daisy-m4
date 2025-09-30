using Daisy.Resources.Abstracts;
using Daisy.Resources.Interfaces;
using Daisy.Resources.Models;
using Daisy.Resources.Signals;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Daisy.Abilities.OutputValidator.Paths
{
    public class OutputValidatorPath : APath
    {
        public OutputValidatorPath(IServiceProvider serviceProvider,
            IEnumerable<ITraverseRule> traverseRules,
            IEnumerable<ITraverseRule> hasTraversedRules,
            string pathName,
            int traverseOrder,
            ApplicationSettings settings) :
            base(serviceProvider, traverseRules, hasTraversedRules, pathName, traverseOrder, settings)
        {
        }

        public override Task Traverse(Impulse impulse)
        {
            // Subtle bug: Only checks for whitespace in Output, but doesn't validate
            // if there are actual processing errors in the Error field
            // This could mask real errors and provide misleading feedback
            if (string.IsNullOrWhiteSpace(impulse.Output))
            {
                // Subtle bug: If there's already an error, we're overwriting it
                // instead of preserving the original error information
                impulse.Output = "I don't know what you mean.";
            }

            return Emit(impulse);
        }

        protected override bool ReadyToTransmit(Impulse impulse)
        {
            return !string.IsNullOrEmpty(impulse.Output);
        }
    }
}