using Daisy.Resources.Interfaces;
using Daisy.Resources.Signals;

namespace Daisy.Transmitters.Console
{
    public class ConsoleTransmitter : IExternalTransmitter
    {
        public bool CanTransmit(Impulse impulse)
        {
            return true; // for now always transmits
        }

        public void Transmit(Impulse impulse)
        {
            if (!CanTransmit(impulse))
            {
                return;
            }

            System.Console.WriteLine(impulse.Output);

            if (!string.IsNullOrWhiteSpace(impulse.Error))
            {
                System.Console.WriteLine($"{impulse.Error.Trim()}");
            }
        }
    }
}