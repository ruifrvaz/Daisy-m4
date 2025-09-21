using Daisy.Resources.Signals;
using System.Threading.Tasks;

namespace Daisy.Resources.Interfaces
{
    /// <summary>
    /// Defines the contract for all paths in the Daisy workflow orchestration engine.
    /// Paths are execution units within Abilities that process and transform Impulse objects
    /// according to traverse rules and ordering constraints.
    /// This is the core interface that enables the rule-driven traversal architecture.
    /// </summary>
    public interface IPath
    {
        /// <summary>
        /// Gets the execution priority order for this path.
        /// Lower numbers execute first, enabling deterministic path ordering.
        /// </summary>
        int TraverseOrder { get; }

        /// <summary>
        /// Gets the unique identifier name for this path.
        /// Used for logging, debugging, and configuration reference.
        /// </summary>
        string PathName { get; }

        /// <summary>
        /// Determines whether this path can be traversed with the given impulse.
        /// Evaluates traverse rules to decide if execution conditions are met.
        /// </summary>
        /// <param name="impulse">The impulse object to evaluate against traverse rules</param>
        /// <returns>True if the path can be traversed, false otherwise</returns>
        bool CanTraverse(Impulse impulse);

        /// <summary>
        /// Determines whether this path has already been traversed for the given impulse.
        /// Prevents re-processing by checking against HasBeenTraversed rules.
        /// </summary>
        /// <param name="impulse">The impulse object to check for previous traversal</param>
        /// <returns>True if the path has been traversed, false otherwise</returns>
        bool HasBeenTraversed(Impulse impulse);

        /// <summary>
        /// Executes the core logic of this path, processing the impulse.
        /// This method contains the main business logic for transforming or enriching the impulse.
        /// </summary>
        /// <param name="impulse">The impulse object to process</param>
        /// <returns>A task representing the asynchronous traversal operation</returns>
        Task Traverse(Impulse impulse);

        /// <summary>
        /// Emits the impulse to the next stage in the workflow after processing.
        /// This method should be called in all traverses to ensure proper workflow continuation.
        /// </summary>
        /// <param name="impulse">The processed impulse to emit</param>
        /// <returns>A task representing the asynchronous emission operation</returns>
        Task Emit(Impulse impulse);
    }
}