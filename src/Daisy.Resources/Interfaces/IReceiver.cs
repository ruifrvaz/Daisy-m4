using Daisy.Resources.Signals;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Daisy.Resources.Interfaces
{
    public interface IReceiver : ITask
    {
        public IEnumerable<string> RunOnCores { get; }

        Task<Impulse> ReceiveAsync();

        Impulse Receive();
    }
}
