using Daisy.Resources.Abstracts;
using Daisy.Resources.Extensions;
using Daisy.Resources.Helpers;
using Daisy.Resources.Interfaces;
using Daisy.Resources.Models;
using Daisy.Resources.Signals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Daisy.Abilities.Operator.Paths
{
    public class WorkflowTriggerPath : APath
    {
        public WorkflowTriggerPath(
            IServiceProvider serviceProvider,
            IEnumerable<ITraverseRule> traverseRules,
            IEnumerable<ITraverseRule> hasBeenTraversedRules,
            string pathName,
            int traverseOrder,
            ApplicationSettings settings)
            : base(serviceProvider, traverseRules, hasBeenTraversedRules, pathName, traverseOrder, settings)
        {
        }

        public override Task Traverse(Impulse impulse)
        {
            var availableWorkflows = WorkflowFinder.GetAvailableWorkflowIdentifiers();

            if (!TryExtractWorkflowRequest(impulse.Input, availableWorkflows, out var workflowIdentifier, out var workflowParameters))
            {
                return Emit(impulse);
            }

            impulse.Input = string.Empty;

            impulse.AddChain($"workflowIdentifier: {workflowIdentifier}");

            var workflowInvocationChain = BuildWorkflowInvocationChain(workflowIdentifier, workflowParameters);
            impulse.AddChain(workflowInvocationChain);

            impulse.IsLoopback = true;

            impulse.Output = string.IsNullOrWhiteSpace(workflowParameters)
                ? $"Triggering {workflowIdentifier} workflow."
                : $"Triggering {workflowIdentifier} workflow with \"{workflowParameters}\".";

            return Emit(impulse);
        }

        

        internal static bool TryExtractWorkflowRequest(
            string input,
            IReadOnlyCollection<string> availableWorkflowIdentifiers,
            out string workflowIdentifier,
            out string workflowParameters)
        {
            workflowIdentifier = string.Empty;
            workflowParameters = string.Empty;

            if (string.IsNullOrWhiteSpace(input) || availableWorkflowIdentifiers == null || availableWorkflowIdentifiers.Count == 0)
            {
                return false;
            }

            var trimmedInput = input.Trim();

            if (TryParseWithDelimiter(trimmedInput, availableWorkflowIdentifiers, out workflowIdentifier, out workflowParameters))
            {
                return true;
            }

            foreach (var candidate in availableWorkflowIdentifiers)
            {
                if (!trimmedInput.StartsWith(candidate, StringComparison.InvariantCultureIgnoreCase))
                {
                    continue;
                }

                workflowIdentifier = candidate;
                workflowParameters = trimmedInput[candidate.Length..].TrimStart(' ', ':').Trim();
                return true;
            }

            return false;
        }

        public static string BuildWorkflowInvocationChain(string workflowIdentifier, string workflowParameters)
        {
            if (string.IsNullOrWhiteSpace(workflowIdentifier))
            {
                return string.Empty;
            }

            if (string.IsNullOrWhiteSpace(workflowParameters))
            {
                return $"{workflowIdentifier}:";
            }

            return $"{workflowIdentifier}: {workflowParameters}";
        }

        private static bool TryParseWithDelimiter(
            string input,
            IReadOnlyCollection<string> availableWorkflowIdentifiers,
            out string workflowIdentifier,
            out string workflowParameters)
        {
            workflowIdentifier = string.Empty;
            workflowParameters = string.Empty;

            var delimiterIndex = input.IndexOf(':');
            if (delimiterIndex < 0)
            {
                return false;
            }

            var candidate = input[..delimiterIndex].Trim();
            var match = availableWorkflowIdentifiers.FirstOrDefault(identifier => identifier.Equals(candidate, StringComparison.InvariantCultureIgnoreCase));

            if (string.IsNullOrEmpty(match))
            {
                return false;
            }

            workflowIdentifier = match;
            workflowParameters = delimiterIndex < input.Length - 1
                ? input[(delimiterIndex + 1)..].Trim()
                : string.Empty;

            return true;
        }
    }
}

