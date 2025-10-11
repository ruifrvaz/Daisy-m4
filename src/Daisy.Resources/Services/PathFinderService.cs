using System;
using System.Linq;
using Daisy.Resources.Interfaces;
using Daisy.Resources.Models;
using Daisy.Resources.Pools;
using Daisy.Resources.Signals;

namespace Daisy.Resources.Services
{
    /// <summary>
    /// Service responsible for finding the next eligible path for impulse traversal in the Daisy workflow engine.
    /// This service implements the core path selection logic that drives the rule-based workflow orchestration.
    /// It evaluates all available paths against traverse rules, traversed rules, and ordering constraints
    /// to determine the optimal execution path for each impulse.
    /// </summary>
    public class PathFinderService : IPathFinder
    {
        /// <summary>
        /// Initializes a new instance of the PathFinderService class.
        /// This constructor is required for the Daisy service loading pattern.
        /// </summary>
        /// <param name="settings">The application settings (not used by PathFinderService but required by the loading pattern)</param>
        public PathFinderService(ApplicationSettings settings)
        {
            // PathFinderService doesn't need settings, but the constructor signature is required for service loading
        }

        /// <summary>
        /// Initializes the service with the provided service provider for dependency injection.
        /// PathFinderService doesn't require any dependencies from the service provider.
        /// </summary>
        /// <param name="serviceProvider">The service provider containing registered dependencies</param>
        public void Initialize(IServiceProvider serviceProvider)
        {
            // No initialization required - PathFinderService is stateless
        }

        /// <summary>
        /// Finds the next eligible path that the impulse can traverse.
        /// 
        /// The selection algorithm works as follows:
        /// 1. If the impulse has already traversed paths:
        ///    - Consider only paths with TraverseOrder >= last traversed path's order
        ///    - This ensures forward progression through the workflow
        /// 2. If the impulse has not traversed any paths:
        ///    - Consider all available paths
        /// 3. Filter paths by:
        ///    - CanTraverse: All traverse rules must pass
        ///    - Not Traversed: Path must not have been processed already
        /// 4. Order by TraverseOrder (ascending) and return the first match
        /// 
        /// This ensures deterministic, ordered execution while respecting workflow rules.
        /// </summary>
        /// <param name="impulse">The impulse object to find the next path for</param>
        /// <returns>The next path to traverse, or null if no eligible paths are available</returns>
        public IPath FindNextPathToTraverse(Impulse impulse)
        {
            IPath nextPathToTraverse;
            
            if (impulse.TraversedPaths.Any())
            {
                // Continue from where we left off - only consider paths at or after the current position
                nextPathToTraverse = Paths.Instance.Pool
                    .Where(path => path.TraverseOrder >= impulse.TraversedPaths.Last().TraverseOrder)
                    .OrderBy(path => path.TraverseOrder)
                    .FirstOrDefault(path => path.CanTraverse(impulse) && !path.Traversed(impulse));
            }
            else
            {
                // Starting fresh - consider all paths from the beginning
                nextPathToTraverse = Paths.Instance.Pool
                    .Where(path => path.CanTraverse(impulse) && !path.Traversed(impulse))
                    .OrderBy(path => path.TraverseOrder)
                    .FirstOrDefault();
            }

            return nextPathToTraverse;
        }
    }
}
