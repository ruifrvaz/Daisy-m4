using System;
using System.Collections.Generic;
using Daisy.Resources.Interfaces;

namespace Daisy.Resources.Pools
{
    public sealed class LoopBackTransmitters : IPool<ILoopBackTransmitter>
    {
        public List<ILoopBackTransmitter> Pool { get; set; }
        private static LoopBackTransmitters _instance;

        /// <inheritdoc />
        private LoopBackTransmitters()
        {
            Pool = new List<ILoopBackTransmitter>();
        }

        public static LoopBackTransmitters Instance
        {
            get
            {
                return _instance ?? (_instance = new LoopBackTransmitters());
            }
        }
    }
}