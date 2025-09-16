using Daisy.Resources.Abstracts;
using Daisy.Resources.Extensions;
using Daisy.Resources.Interfaces;
using Daisy.Resources.Models;
using Daisy.Resources.Pools;
using Daisy.Resources.Signals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Daisy.Resources.Extensions.ImpulseExtensions;

namespace Daisy.Abilities.Start
{
    public class WorkflowsPath : APath
    {
        public WorkflowsPath(IServiceProvider serviceProvider,
            IEnumerable<ITraverseRule> traverseRules,
            IEnumerable<ITraverseRule> hasBeenTraversedRules,
            string pathName,
            int traverseOrder,
            ApplicationSettings settings) : base(serviceProvider, traverseRules, hasBeenTraversedRules, pathName, traverseOrder, settings) { }

        public override Task Traverse(Impulse impulse)
        {
            impulse.AddChain($"WorkflowsPath: {GenerateOptions()}", ImpulseField.Output);

            return Emit(impulse);
        }

        private string GenerateOptions()
        {
            var stringBuilder = new StringBuilder();

            var workflows = Cores.Instance.Pool.Where(workflow => !workflow.GetType().Namespace.Contains("starter", StringComparison.InvariantCultureIgnoreCase))
                                               .Select(workflow => workflow.GetType().Namespace.RemoveSubstring("Daisy.Workflows.", true))
                                               .ToList();
            if (!workflows.Any())
            {
                stringBuilder.AppendLine("There are currently no workflows available");
            }
            else
            {
                stringBuilder.AppendLine("These are the currently active workflows");
                foreach (var workflow in workflows)
                {
                    stringBuilder.AppendLine(workflow);
                }
            }

            return stringBuilder.ToString();
        }
    }
}
