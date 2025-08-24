using System;
using Daisy.Resources.Signals;

namespace Daisy.Resources.Interfaces
{
    // This is the interface that will be implemented by all services that transmit outputs
    public interface IExternalTransmitter
    {
        bool CanTransmit(Impulse impulse);

        void Transmit(Impulse impulse);
    }
}