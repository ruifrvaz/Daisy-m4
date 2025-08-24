using Daisy.Resources.Signals;
using System.Threading.Tasks;

namespace Daisy.Resources.Interfaces
{
    // This is the Interface that will be implemented by all instructions run by Daisy
    public interface IPath
    {
        int TraverseOrder { get; }

        string PathName { get; }

        bool CanTraverse(Impulse impulse);

        bool HasBeenTraversed(Impulse impulse);

        Task Traverse(Impulse impulse);

        Task Emit(Impulse impulse);
    }
}