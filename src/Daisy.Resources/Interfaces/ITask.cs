using System.Threading;
using System.Threading.Tasks;

namespace Daisy.Resources.Interfaces
{
    public interface ITask
    {
        bool IsActive { get; set; }
        Task Start(CancellationToken token);
        void Stop();
    }
}