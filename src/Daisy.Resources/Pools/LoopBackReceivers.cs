using Daisy.Resources.Interfaces;
using Daisy.Resources.Signals;
using System.Collections.Generic;

namespace Daisy.Resources.Pools
{
    public sealed class LoopBackReceivers : IPool<ILoopBackReceiver>
    {
        public List<ILoopBackReceiver> Pool { get; set; }

        private static LoopBackReceivers _instance;

        /// <inheritdoc />
        private LoopBackReceivers()
        {

            Pool = new List<ILoopBackReceiver>();
        }

        public static LoopBackReceivers Instance
        {
            get
            {
                return _instance ?? (_instance = new LoopBackReceivers());
            }
        }

        public void DispatchAsync(Impulse impulse)
        {
            foreach (var receiver in Pool)
            {
                receiver.ReceiveLoopBack(impulse);
            }
        }
    }
}