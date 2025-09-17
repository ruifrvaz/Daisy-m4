using Daisy.Resources.Interfaces;
using System.Collections.Generic;

namespace Daisy.Resources.Pools
{
    public sealed class EventReceivers : IPool<IEventReceiver>
    {
        public List<IEventReceiver> Pool { get; set; }

        private static EventReceivers _instance;

        /// <inheritdoc />
        private EventReceivers()
        {
            Pool = new List<IEventReceiver>();
        }

        public static EventReceivers Instance
        {
            get
            {
                return _instance ?? (_instance = new EventReceivers());
            }
        }
    }
}