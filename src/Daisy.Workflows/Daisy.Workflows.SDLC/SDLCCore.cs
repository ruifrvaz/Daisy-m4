using Daisy.Resources.Abstracts;
using Daisy.Resources.Pools;
using System.Threading.Tasks;
using System.Threading;
using System;
using System.Linq;
using Daisy.Resources.Interfaces;

namespace Daisy.Workflows.SDLC
{
    public class SDLCCore : ACore
    {
        public override async Task Start(CancellationToken token)
        {
            IsActive = true;

            string coreNamespace = GetType().Namespace ?? throw new InvalidOperationException("Core has no namespace.");

            _receivers = EventReceivers.Instance.Pool
               .Where(r => r.RunOnCores.Contains(coreNamespace, StringComparer.OrdinalIgnoreCase))
               .Cast<IReceiver>()
               .ToList();

            var receiverTasks = _receivers.Select(receiver => receiver.Start(token));

            await Task.WhenAll(receiverTasks);
        }
    }
}