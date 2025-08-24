using Daisy.Resources.Signals;

namespace Daisy.Resources.Interfaces
{
    public interface ITraverseRule
    {
        bool RuleApplies(Impulse impulse);
    }
}