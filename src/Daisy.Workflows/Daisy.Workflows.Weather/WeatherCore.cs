using Daisy.Resources.Abstracts;
using Daisy.Resources.Interfaces;
using Daisy.Resources.Pools;

namespace Daisy.Workflows.Weather
{
    public class WeatherCore : ACore
    {
        public override Task Start(CancellationToken token)
        {
            IsActive = true;

            string coreNamespace = GetType().Namespace ?? throw new InvalidOperationException("Core has no namespace.");

            _receivers = EventReceivers.Instance.Pool
               .Where(r => r.RunOnCores.Contains(coreNamespace, StringComparer.OrdinalIgnoreCase))
               .Cast<IReceiver>()
               .ToList();

            var receiverTasks = _receivers
                .Select(receiver => Task.Run(() => receiver.Start(token), token))
                .ToList();

            return Task.WhenAll(receiverTasks);
        }
    }
}