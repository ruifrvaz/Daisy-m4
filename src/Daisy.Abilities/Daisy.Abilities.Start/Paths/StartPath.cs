using Daisy.Resources.Abstracts;
using Daisy.Resources.Extensions;
using Daisy.Resources.Interfaces;
using Daisy.Resources.Models;
using Daisy.Resources.Pools;
using Daisy.Resources.Signals;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Daisy.Abilities.Start
{
    public class StartPath : APath
    {
        public StartPath(IServiceProvider serviceProvider, 
            IEnumerable<ITraverseRule> traverseRules, 
            IEnumerable<ITraverseRule> hasBeenTraversedRules,
            string pathName,
            int traverseOrder,
            ApplicationSettings settings) 
            : base(serviceProvider, traverseRules, hasBeenTraversedRules, pathName, traverseOrder, settings)
        {
            Console.WriteLine($"Type \"start\" to list workflows. Type \"bye\" to terminate.");
        }

        public override Task Traverse(Impulse impulse)
        {
            impulse.Output = GenerateStartOptions();
            return Emit(impulse);
        }

        private string GenerateStartOptions()
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("These are the currently active workflows:");
            for (int i = 0; i < Cores.Instance.Pool.Count; i++)
            {
                stringBuilder.AppendLine(Cores.Instance.Pool[i].GetType().Namespace.RemoveSubstring("Daisy.Workflows.", true));
            }
            
            return stringBuilder.ToString();
        }
    }
}
