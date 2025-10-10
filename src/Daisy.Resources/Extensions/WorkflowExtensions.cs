using System;
using System.Collections.Generic;
using System.Linq;

namespace Daisy.Resources.Extensions
{
    public static class WorkflowExtensions
    {
        /// <summary>
        /// Checks if a workflow identifier matches any of the RunOnCores entries.
        /// Compares only the last segment of the namespace (e.g., "Weather" matches "Daisy.Workflows.Weather")
        /// </summary>
        /// <param name="runOnCores">Collection of full workflow namespaces</param>
        /// <param name="workflowIdentifier">Short workflow identifier (e.g., "Weather", "Flights")</param>
        /// <returns>True if the identifier matches any RunOnCores entry</returns>
        public static bool ContainsWorkflowIdentifier(this IEnumerable<string> runOnCores, string workflowIdentifier)
        {
            if (runOnCores == null || string.IsNullOrWhiteSpace(workflowIdentifier))
            {
                return false;
            }

            return runOnCores.Any(core =>
                core.Split('.', StringSplitOptions.RemoveEmptyEntries)
                    .LastOrDefault()
                    ?.Equals(workflowIdentifier, StringComparison.OrdinalIgnoreCase) == true);
        }
    }
}