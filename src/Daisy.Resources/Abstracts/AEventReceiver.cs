using Daisy.Resources.Helpers;
using Daisy.Resources.Interfaces;
using Daisy.Resources.Pools;
using Daisy.Resources.Signals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Daisy.Resources.Abstracts
{
    public abstract class AEventReceiver : IEventReceiver
    {
        public abstract IEnumerable<string> RunOnCores { get; }

        public bool IsActive { get; set; }

        public abstract void RaiseEvent(Impulse input);

        public abstract Impulse Receive();

        public abstract Task<Impulse> ReceiveAsync();

        public virtual async Task Start(CancellationToken token)
        {
            IsActive = true;
            var nrExceptions = 0;
            do
            {
                try
                {
                    var impulse = await ReceiveAsync();
                    var nextPath = PathFinder.FindNextPathToTraverse(impulse);
                    if (nextPath != null)
                    {
                        impulse.TraversedPaths.Enqueue(nextPath);
                        await impulse.TraversedPaths.Last().Traverse(impulse);
                    }
                    else
                    {
                        Parallel.ForEach(ExternalTransmitters.Instance.Pool, transmitter => transmitter.Transmit(impulse));
                    }
                }
                catch (Exception ex)
                {
                    System.Console.WriteLine($"{this}, {ex}");
                    nrExceptions++;
                }
            } while (IsActive && nrExceptions < 3);
        }

        public virtual void Stop()
        {
            if (IsActive)
            {
                IsActive = false;
            }
        }
    }
}
