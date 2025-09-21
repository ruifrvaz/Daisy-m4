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
            if (string.IsNullOrWhiteSpace(impulse.Output))
            {
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