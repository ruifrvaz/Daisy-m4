using System;
using System.Collections.Generic;
using Daisy.Resources.Interfaces;

namespace Daisy.Resources.Signals
{
    /// <summary>
    /// Represents the central data object that carries information and state throughout the Daisy workflow engine.
    /// The Impulse is the fundamental signal that travels through the entire workflow cycle, accumulating
    /// state and history as it passes through receivers, abilities, and transmitters.
    /// 
    /// Key characteristics:
    /// - Serves as the only data contract between workflow components
    /// - Carries serialized data and metadata throughout processing
    /// - Maintains processing history and path traversal information
    /// - Enables fine-tuning of workflows without breaking component contracts
    /// - Supports error propagation and handling across the workflow
    /// </summary>
    public class Impulse
    {
        /// <summary>
        /// Gets or sets the queue of paths that have been traversed during impulse processing.
        /// This maintains the execution history and enables the workflow engine to track
        /// which processing steps have been completed for this impulse.
        /// </summary>
        public Queue<IPath> TraversedPaths { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this impulse is being processed in a loop-back scenario.
        /// Loop-back processing occurs when impulses are sent back into the workflow system
        /// for additional processing cycles, iteration, or recursive processing patterns.
        /// </summary>
        public bool IsLoopback { get; set; }

        /// <summary>
        /// Gets or sets the input data for this impulse.
        /// Contains the raw or processed input data that initiated the workflow
        /// or was received from external sources. This data flows through the workflow
        /// and can be transformed by various abilities during processing.
        /// </summary>
        public string Input { get; set; }

        /// <summary>
        /// Gets or sets the output data produced by workflow processing.
        /// Contains the results of workflow processing, including transformed data,
        /// computed results, or formatted responses. This is typically what gets
        /// transmitted to external systems or users at the end of processing.
        /// </summary>
        public string Output { get; set; }

        /// <summary>
        /// Gets or sets error information if processing failures occur.
        /// Contains error details, exception messages, or failure information
        /// encountered during workflow processing. Non-empty error values
        /// typically trigger error handling or recovery mechanisms.
        /// </summary>
        public string Error { get; set; }

        /// <summary>
        /// Initializes a new instance of the Impulse class with default values.
        /// Creates an empty impulse ready for workflow processing with:
        /// - Empty input, output, and error strings
        /// - IsLoopback set to false
        /// - Empty traversed paths queue
        /// </summary>
        public Impulse()
        {
            Input = string.Empty;
            Output = string.Empty;
            IsLoopback = false;
            Error = string.Empty;
            TraversedPaths = new Queue<IPath>();
        }
    }
}