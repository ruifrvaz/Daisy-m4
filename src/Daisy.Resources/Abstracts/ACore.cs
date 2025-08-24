using Daisy.Resources.Interfaces;
using System.Threading;
using System.Threading.Tasks;
using Daisy.Resources.Pools;
using System.Linq;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Daisy.Resources.Abstracts
{
    // The Core is the main thread that will start the workflows.
    // These are the only threads that run.
    // The paths are not threads. They are just channels that modify and conduct the impulses to the transmitters. 
    public abstract class ACore : ICore // create a IWorkflow core that loads event receivers
    {
        public bool IsActive { get; set; }

        public virtual List<IReceiver> _receivers { get; set; }

        public virtual async Task Start(CancellationToken token)
        {
            IsActive = true;

            string coreNamespace = GetType().Namespace ?? throw new InvalidOperationException("Core has no namespace.");

            _receivers = ExternalReceivers.Instance.Pool
                .Where(r => r.RunOnCores.Contains(coreNamespace, StringComparer.OrdinalIgnoreCase))
                .Cast<IReceiver>()
                .ToList();

            var receiverTasks = _receivers.Select(receiver => receiver.Start(token));

            await Task.WhenAll(receiverTasks);
        }

        public virtual void Stop()
        {
            if (IsActive)
            {
                _receivers.ForEach(receiver => receiver.Stop());
                IsActive = false;
            }
        }
    }
}