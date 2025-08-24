using System;
using System.Collections.Generic;
using Daisy.Resources.Interfaces;

namespace Daisy.Resources.Pools
{
    public sealed class Cores : IPool<ICore>
    {
        public List<ICore> Pool { get; set; }
        private static Cores _instance;

        private Cores()
        {
            Pool = new List<ICore>();
        }

        public static Cores Instance
        {
            get
            {
                return _instance ??= new Cores();
            }
        }
    }
}