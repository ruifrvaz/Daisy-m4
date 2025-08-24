using Daisy.Resources.Interfaces;
using System.Collections.Generic;

namespace Daisy.Resources.Pools
{
    public sealed class ExternalTransmitters : IPool<IExternalTransmitter>
    {
        public List<IExternalTransmitter> Pool { get; set; }
        private static ExternalTransmitters _instance;

        /// <inheritdoc />
        private ExternalTransmitters()
        {
            Pool = new List<IExternalTransmitter>();
        }

        public static ExternalTransmitters Instance
        {
            get
            {
                return _instance ?? (_instance = new ExternalTransmitters());
            }
        }
    }
}