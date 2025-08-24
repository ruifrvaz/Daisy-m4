using Daisy.Resources.Interfaces;
using System.Collections.Generic;

namespace Daisy.Resources.Pools
{
    public sealed class ExternalReceivers : IPool<IExternalReceiver>
    {
        public List<IExternalReceiver> Pool { get; set; }

        private static ExternalReceivers _instance;

        /// <inheritdoc />
        private ExternalReceivers()
        {
            Pool = new List<IExternalReceiver>();
        } 

        public static ExternalReceivers Instance
        {
            get
            {
                return _instance ?? (_instance = new ExternalReceivers());
            }
        }
    }
}