using Daisy.Resources.Abstracts;
using Daisy.Resources.Extensions;
using Daisy.Resources.Interfaces;
using Daisy.Resources.Models;
using Daisy.Resources.Signals;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static Daisy.Resources.Extensions.ImpulseExtensions;

namespace Daisy.Abilities.Start
{
    public class WorkflowTriggerPath : APath
    {
        public WorkflowTriggerPath(IServiceProvider serviceProvider,
            IEnumerable<ITraverseRule> traverseRules,
            IEnumerable<ITraverseRule> hasBeenTraversedRules,
            string pathName,
            int traverseOrder,
            ApplicationSettings settings) : base(serviceProvider, traverseRules, hasBeenTraversedRules, pathName, traverseOrder, settings) { }

        public override Task Traverse(Impulse impulse)
        {
            // Process the workflow trigger - for "Weather: Porto", extract city name and add it to the chain
            if (impulse.Input.StartsWith("Weather: ", StringComparison.InvariantCultureIgnoreCase))
            {
                var cityName = impulse.Input.Substring("Weather: ".Length).Trim();
                impulse.AddChain($"cityName:{cityName}", ImpulseField.Input);
                impulse.AddChain($"WorkflowTriggerPath: Triggered workflow for {cityName}", ImpulseField.Output);
            }

            return Emit(impulse);
        }
    }
}