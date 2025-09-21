using Daisy.Resources.Helpers;
using Daisy.Resources.Interfaces;
using Daisy.Resources.Models;
using Daisy.Resources.Pools;
using Daisy.Resources.Signals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Daisy.Resources.Abstracts
{
    // Class that defines the base for all paths. In order to traverse a path, the Traverse rules must be met.
    // In order to completely traverse a path, the hasBeenTraversed rules must be obeyed.
    // This combination of traverse rules and has been traversed rules is one of the logic pillars of daisy's architecture.
    // Every time a path needs to be traversed, Daisy performs two checks (can traverse and has been traversed) for each path, 
    // which allows the paths to be failsafe and self contained.
    public abstract class APath : IPath
    {
        public ApplicationSettings Settings { get; set; }
        public IServiceProvider ServiceProvider { get; set; }
        public IEnumerable<ITraverseRule> TraverseRules { get; set; }
        public IEnumerable<ITraverseRule> HasBeenTraversedRules { get; set; }
        public int TraverseOrder { get; }
        public string PathName { get; }

        public APath(IServiceProvider serviceProvider,
            IEnumerable<ITraverseRule> traverseRules,
            IEnumerable<ITraverseRule> hasBeenTraversedRules,
            string pathName,
            int traverseOrder,
            ApplicationSettings settings)
        {
            ServiceProvider = serviceProvider;
            TraverseRules = traverseRules;
            HasBeenTraversedRules = hasBeenTraversedRules;
            Settings = settings;
            PathName = pathName;
            TraverseOrder = traverseOrder;
        }

        public abstract Task Traverse(Impulse impulse);

        public virtual bool HasBeenTraversed(Impulse impulse)
        {
            return HasBeenTraversedRules.Any(tr => tr.RuleApplies(impulse));
        }

        private bool TransmitLoopBack(Impulse impulse)
        {
            return string.IsNullOrEmpty(impulse.Error) && LoopBackTransmitters.Instance.Pool.Where(tr => tr.CanTransmit(impulse)).Any();
        }

        protected virtual bool ReadyToTransmit(Impulse impulse)
        {
            //TODO: create a recovery mechanism instead of straight out transmitting the error when an ability fails
            return PathFinder.FindNextPathToTraverse(impulse) == null || !string.IsNullOrWhiteSpace(impulse.Error);
        }

        public virtual bool CanTraverse(Impulse impulse)
        {
            return TraverseRules.All(rule => rule.RuleApplies(impulse));
        }

        /// <summary>
        /// This method should be called in all traverses. Otherwise the impulse is not emitted.
        /// </summary>
        /// <param name="impulse"></param>
        public async virtual Task Emit(Impulse impulse)
        {
            if (TransmitLoopBack(impulse))
            {
                Parallel.ForEach(LoopBackTransmitters.Instance.Pool, p => p.TransmitLoopBack(impulse));
            }
            else if (ReadyToTransmit(impulse))
            {
                Parallel.ForEach(ExternalTransmitters.Instance.Pool, p => p.Transmit(impulse));
            }
            else
            {
                impulse.TraversedPaths.Enqueue(PathFinder.FindNextPathToTraverse(impulse));
                await impulse.TraversedPaths.Last().Traverse(impulse);
            }
        }
    }
}