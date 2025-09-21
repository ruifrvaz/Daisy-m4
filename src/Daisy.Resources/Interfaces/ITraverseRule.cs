using Daisy.Resources.Signals;

namespace Daisy.Resources.Interfaces
{
    /// <summary>
    /// Defines the contract for traverse rule components that determine path execution conditions.
    /// Traverse rules are the core logic components that implement the rule-driven architecture
    /// of the Daisy workflow engine. They evaluate whether paths can be traversed or have been
    /// traversed based on impulse state and custom business logic.
    /// </summary>
    public interface ITraverseRule
    {
        /// <summary>
        /// Evaluates whether this rule applies to the given impulse.
        /// This method contains the core business logic for determining path traversal conditions,
        /// such as checking impulse content, state, or metadata against specific criteria.
        /// </summary>
        /// <param name="impulse">The impulse to evaluate against this rule</param>
        /// <returns>True if the rule condition is met, false otherwise</returns>
        bool RuleApplies(Impulse impulse);
    }
}