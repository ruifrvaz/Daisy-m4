using Daisy.Resources.Signals;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Daisy.Resources.Interfaces
{
    public interface IEventReceiver : IReceiver
    {
        void RaiseEvent(Impulse input);
    }
}