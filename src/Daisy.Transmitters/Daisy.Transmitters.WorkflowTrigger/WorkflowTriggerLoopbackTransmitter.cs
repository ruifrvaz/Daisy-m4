using Daisy.Abilities.Operator.Paths;
using Daisy.Resources.Extensions;
using Daisy.Resources.Interfaces;
using Daisy.Resources.Pools;
using Daisy.Resources.Signals;
using System;
using System.Linq;

namespace Daisy.Transmitters.WorkflowTrigger
{
    public class WorkflowTriggerLoopbackTransmitter : ILoopBackTransmitter
    {
        public bool CanTransmit(Impulse impulse)
        {
            if (impulse == null)
            {
                return false;
            }

            var workflowIdentifier = impulse.GetChainByKey("workflowIdentifier");

            return impulse.IsLoopback && !string.IsNullOrWhiteSpace(workflowIdentifier);
        }

        public void TransmitLoopBack(Impulse impulse)
        {
            if (!CanTransmit(impulse))
            {
                return;
            }

            var workflowIdentifier = impulse.GetChainByKey("workflowIdentifier");

            if (string.IsNullOrWhiteSpace(workflowIdentifier))
            {
                return;
            }

            var workflowParameters = impulse.GetChainByKey(workflowIdentifier) ?? string.Empty;
            var eventImpulse = new Impulse();
            var workflowInvocationChain = WorkflowTriggerPath.BuildWorkflowInvocationChain(workflowIdentifier, workflowParameters);

            eventImpulse.AddChain(workflowInvocationChain);

            // Only raise event to receivers that run on the target workflow core
            var targetWorkflowNamespace = $"Daisy.Workflows.{workflowIdentifier}";
            var targetReceivers = EventReceivers.Instance.Pool
                .Where(receiver => receiver.RunOnCores.Contains(targetWorkflowNamespace, StringComparer.OrdinalIgnoreCase))
                .ToList();

            targetReceivers.ForEach(receiver => receiver.RaiseEvent(eventImpulse));
        }
    }
}

