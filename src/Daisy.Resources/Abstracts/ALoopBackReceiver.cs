using Daisy.Resources.Helpers;
using Daisy.Resources.Interfaces;
using Daisy.Resources.Pools;
using Daisy.Resources.Signals;
using System.Linq;
using System.Threading.Tasks;

namespace Daisy.Resources.Abstracts
{
    // Loopback Receivers are dedicated to receiving impulses from the inside.
    // These are usually sent by transmitters and should be pushed into the pool and not directly.
    public abstract class ALoopBackReceiver : ILoopBackReceiver
    {
        public abstract bool CanReceive(Impulse impulse);

        public async virtual Task ReceiveLoopBack(Impulse impulse)
        {
            var nextPath = PathFinder.FindNextPathToTraverse(impulse);
            if (nextPath != null)
            {
                impulse.TraversedPaths.Enqueue(nextPath);
                await impulse.TraversedPaths.Last().Traverse(impulse);
            }
            else if (TransmitLoopBack(impulse))
            {
                Parallel.ForEach(LoopBackTransmitters.Instance.Pool, p => p.TransmitLoopBack(impulse));
            }
            else
            {
                Parallel.ForEach(ExternalTransmitters.Instance.Pool, transmitter => transmitter.Transmit(impulse));
            }
        }

        private bool TransmitLoopBack(Impulse impulse)
        {
            return LoopBackTransmitters.Instance.Pool.Where(tr => tr.CanTransmit(impulse)).Any();
        }
    }
}