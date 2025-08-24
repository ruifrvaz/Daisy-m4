using Daisy.Resources.Abstracts;
using Daisy.Resources.Interfaces;
using Daisy.Resources.Models;
using Daisy.Resources.Pools;
using Daisy.Resources.Signals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Daisy.Abilites.Terminate
{
    public class TerminatePath : APath
    {

        public TerminatePath(IServiceProvider serviceProvider, 
            IEnumerable<ITraverseRule> traverseRules, 
            IEnumerable<ITraverseRule> hasBeenTraversedRules,
            string pathName,
            int traverseOrder,
            ApplicationSettings settings) : base(serviceProvider, traverseRules, hasBeenTraversedRules, pathName, traverseOrder, settings) { }
        
        public override Task Traverse(Impulse impulse)
        {
            var activeCores = Cores.Instance.Pool.Where(core => core.IsActive).ToList();
            activeCores.ForEach(core => core.Stop());

            var eventImpulse = new Impulse
            {
                Output = "CICD workflow terminating."
            };

            EventReceivers.Instance.Pool.ForEach(receiver => receiver.RaiseEvent(eventImpulse));

            impulse.Output = "Goodbye.";

            return Emit(impulse);
        }
    }
}