using Daisy.Resources.Signals;

namespace Daisy.Resources.Interfaces
{
    /// <summary>
    /// Defines the contract for the path finder service in the Daisy workflow orchestration engine.
    /// The path finder is responsible for determining the next eligible path that an impulse should traverse
    /// based on traverse rules, traversed rules, and path ordering constraints.
    /// This is a core service that enables the rule-driven workflow architecture.
    /// </summary>
    public interface IPathFinder
    {
        /// <summary>
        /// Finds the next eligible path that the impulse can traverse.
        /// This method evaluates all available paths against the impulse to determine which path
        /// should be executed next based on:
        /// - Path traverse rules (CanTraverse)
        /// - Path traversed rules (not already processed)
        /// - Path traverse order (execution priority)
        /// </summary>
        /// <param name="impulse">The impulse object to find the next path for</param>
        /// <returns>The next path to traverse, or null if no eligible paths are available</returns>
        IPath FindNextPathToTraverse(Impulse impulse);
    }
}
