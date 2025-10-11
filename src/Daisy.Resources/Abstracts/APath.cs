using Daisy.Resources.Interfaces;
using Daisy.Resources.Models;
using Daisy.Resources.Pools;
using Daisy.Resources.Signals;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Daisy.Resources.Abstracts
{
    /// <summary>
    /// Abstract base class that defines the foundation for all path implementations in the Daisy workflow engine.
    /// Paths are execution units within Abilities that process and transform Impulse objects according to
    /// traverse rules and ordering constraints. This class implements the core rule-driven traversal architecture
    /// by providing traverse and Traversed rule evaluation, making paths failsafe and self-contained.
    /// 
    /// The combination of traverse rules and Traversed rules is a fundamental pillar of Daisy's architecture.
    /// Every path traversal performs dual validation: first checking if traversal is allowed (CanTraverse),
    /// then verifying the path hasn't been processed already (Traversed).
    /// </summary>
    public abstract class APath : IPath
    {
        /// <summary>
        /// Gets or sets the application settings configuration.
        /// Provides access to global configuration data including API settings and module configurations.
        /// </summary>
        public ApplicationSettings Settings { get; set; }

        /// <summary>
        /// Gets or sets the service provider for dependency injection.
        /// Enables access to registered services and dependencies within path implementations.
        /// </summary>
        public IServiceProvider ServiceProvider { get; set; }

        /// <summary>
        /// Gets or sets the collection of traverse rules that determine if this path can be executed.
        /// All traverse rules must pass for the path to be eligible for traversal.
        /// </summary>
        public IEnumerable<ITraverseRule> TraverseRules { get; set; }

        /// <summary>
        /// Gets or sets the collection of rules that determine if this path has already been traversed.
        /// Prevents re-processing by checking if any Traversed rule applies to the impulse.
        /// </summary>
        public IEnumerable<ITraverseRule> TraversedRules { get; set; }

        /// <summary>
        /// Gets the execution priority order for this path.
        /// Lower numbers execute first, enabling deterministic path ordering within abilities.
        /// </summary>
        public int TraverseOrder { get; }

        /// <summary>
        /// Gets the unique identifier name for this path.
        /// Used for logging, debugging, and configuration reference.
        /// </summary>
        public string PathName { get; }

        /// <summary>
        /// Initializes a new instance of the APath class with the specified configuration.
        /// Sets up the path with necessary dependencies, rules, and configuration data.
        /// </summary>
        /// <param name="serviceProvider">The service provider for dependency injection</param>
        /// <param name="traverseRules">The rules that determine if this path can be traversed</param>
        /// <param name="traversedRules">The rules that determine if this path has been traversed</param>
        /// <param name="pathName">The unique name identifier for this path</param>
        /// <param name="traverseOrder">The execution priority order (lower executes first)</param>
        /// <param name="settings">The application settings configuration</param>
        public APath(IServiceProvider serviceProvider,
            IEnumerable<ITraverseRule> traverseRules,
            IEnumerable<ITraverseRule> traversedRules,
            string pathName,
            int traverseOrder,
            ApplicationSettings settings)
        {
            ServiceProvider = serviceProvider;
            TraverseRules = traverseRules;
            TraversedRules = traversedRules;
            Settings = settings;
            PathName = pathName;
            TraverseOrder = traverseOrder;
        }

        /// <summary>
        /// Abstract method that must be implemented by derived classes to define the path's core processing logic.
        /// This method contains the main business logic for transforming or enriching the impulse.
        /// </summary>
        /// <param name="impulse">The impulse object to process</param>
        /// <returns>A task representing the asynchronous traversal operation</returns>
        public abstract Task Traverse(Impulse impulse);

        /// <summary>
        /// Determines whether this path has already been traversed for the given impulse.
        /// Evaluates all Traversed rules - if any rule applies, the path is considered traversed.
        /// </summary>
        /// <param name="impulse">The impulse to check for previous traversal</param>
        /// <returns>True if any Traversed rule applies, false otherwise</returns>
        public virtual bool Traversed(Impulse impulse)
        {
            return TraversedRules.Any(tr => tr.RuleApplies(impulse));
        }

        /// <summary>
        /// Determines whether the impulse should be transmitted via loop-back transmitters.
        /// Loop-back transmission occurs when there are no errors and loop-back transmitters can handle the impulse.
        /// </summary>
        /// <param name="impulse">The impulse to evaluate for loop-back transmission</param>
        /// <returns>True if loop-back transmission is appropriate, false otherwise</returns>
        private bool TransmitLoopBack(Impulse impulse)
        {
            return string.IsNullOrEmpty(impulse.Error) && LoopBackTransmitters.Instance.Pool.Where(tr => tr.CanTransmit(impulse)).Any();
        }

        /// <summary>
        /// Determines whether the impulse is ready for external transmission.
        /// Transmission occurs when no more paths are available for traversal or when an error has occurred.
        /// </summary>
        /// <param name="impulse">The impulse to evaluate for transmission readiness</param>
        /// <returns>True if ready for external transmission, false otherwise</returns>
        protected virtual bool ReadyToTransmit(Impulse impulse)
        {
            //TODO: create a recovery mechanism instead of straight out transmitting the error when an ability fails
            var pathFinder = ServiceProvider.GetRequiredService<IPathFinder>();
            return pathFinder.FindNextPathToTraverse(impulse) == null || !string.IsNullOrWhiteSpace(impulse.Error);
        }

        /// <summary>
        /// Determines whether this path can be traversed with the given impulse.
        /// All traverse rules must pass (return true) for the path to be traversable.
        /// </summary>
        /// <param name="impulse">The impulse to evaluate against traverse rules</param>
        /// <returns>True if all traverse rules pass, false otherwise</returns>
        public virtual bool CanTraverse(Impulse impulse)
        {
            return TraverseRules.All(rule => rule.RuleApplies(impulse));
        }

        /// <summary>
        /// Emits the impulse to the next appropriate destination in the workflow.
        /// This method implements the core workflow routing logic, determining whether to:
        /// 1. Send to loop-back transmitters for internal processing
        /// 2. Send to external transmitters for final output
        /// 3. Continue to the next path in the current workflow
        /// 
        /// This method should be called in all path traversals to ensure proper workflow continuation.
        /// </summary>
        /// <param name="impulse">The processed impulse to emit</param>
        /// <returns>A task representing the asynchronous emission operation</returns>
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
                var pathFinder = ServiceProvider.GetRequiredService<IPathFinder>();
                impulse.TraversedPaths.Enqueue(pathFinder.FindNextPathToTraverse(impulse));
                await impulse.TraversedPaths.Last().Traverse(impulse);
            }
        }
    }
}