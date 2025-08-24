using Daisy.Resources.Signals;
using System.Threading.Tasks;

namespace Daisy.Resources.Interfaces
{
    public interface ILoopBackReceiver
    {
        public bool CanReceive(Impulse impulse);

        public Task ReceiveLoopBack(Impulse impulse);
    }
}
