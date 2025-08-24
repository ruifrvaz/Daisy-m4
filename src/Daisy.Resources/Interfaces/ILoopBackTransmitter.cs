using Daisy.Resources.Signals;

namespace Daisy.Resources.Interfaces
{
    public interface ILoopBackTransmitter
    {
        bool CanTransmit(Impulse impulse);

        void TransmitLoopBack(Impulse impulse);
    }
}
