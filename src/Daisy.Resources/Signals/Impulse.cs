using System;
using System.Collections.Generic;
using Daisy.Resources.Interfaces;

namespace Daisy.Resources.Signals
{
    // Impulse is the signal that travels through the cycle.
    // what is absolutely essential is that this is the only data object. it will carry all the serialized data and metadata.
    // this way we can fine tune agents without breaking the contract all the time.
    public class Impulse
    {
        public Queue<IPath> TraversedPaths { get; set; }

        public bool IsLoopback { get; set; }

        public string Input { get; set; }

        public string Output { get; set; }

        public string Error { get; set; }

        public Impulse()
        {
            Input = string.Empty;
            Output = string.Empty;
            IsLoopback = false;
            Error = string.Empty;
            TraversedPaths = new Queue<IPath>();
        }
    }
}