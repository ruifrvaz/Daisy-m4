using System;

namespace Daisy.Resources.Attributes
{
    /// <summary>
    /// Attribute that marks classes as "HasBeenTraversed" rule providers for specific path types.
    /// This attribute is used to establish relationships between rule classes and the paths
    /// they prevent from re-processing, enabling the dependency injection system to automatically
    /// wire HasBeenTraversed rules to their corresponding paths during workflow initialization.
    /// 
    /// Can be applied multiple times to a single class to support multiple path types.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class TraversedRuleAttribute : Attribute
    {
        /// <summary>
        /// Gets or sets the type of path that this HasBeenTraversed rule applies to.
        /// Specifies which path class should use this rule to prevent re-processing.
        /// The dependency injection system uses this information to automatically
        /// register the rule with the appropriate path implementations for duplicate execution prevention.
        /// </summary>
        public Type PathType { get; set; }
    }
}