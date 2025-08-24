using System;
using System.Collections.Generic;
using Daisy.Resources.Interfaces;

namespace Daisy.Resources.Pools
{
    public sealed class Paths : IPool<IPath>
    {
        public List<IPath> Pool { get; set; }
        private static Paths _instance;

        private Paths()
        {
            Pool = new List<IPath>();
        }

        public static Paths Instance
        {
            get
            {
                return _instance ??= new Paths();
            }
        }
    }
}