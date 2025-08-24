using System.Linq;
using Daisy.Resources.Interfaces;
using Daisy.Resources.Pools;
using Daisy.Resources.Signals;

namespace Daisy.Resources.Helpers
{
    // The path finder will return the next path that the impulse can traverse.
    public static class PathFinder
    {
        public static IPath FindNextPathToTraverse(Impulse impulse)
        {
            IPath nextPathToTraverse;
            if (impulse.TraversedPaths.Any())
            {
                nextPathToTraverse = Paths.Instance.Pool.Where(path => path.TraverseOrder >= impulse.TraversedPaths.Last().TraverseOrder)
                                                        .OrderBy(path => path.TraverseOrder)
                                                        .FirstOrDefault(path => path.CanTraverse(impulse) && !path.HasBeenTraversed(impulse));
            }
            else
            {
                nextPathToTraverse = Paths.Instance.Pool.Where(path => path.CanTraverse(impulse) && !path.HasBeenTraversed(impulse))
                                                        .OrderBy(path => path.TraverseOrder)
                                                        .FirstOrDefault();
            }

            return nextPathToTraverse;
        }
    }
}