using System;

namespace Daisy.Resources.Attributes
{
    /// <summary>
    /// Attribute that specifies which workflow cores a receiver should be active on.
    /// This attribute enables targeted deployment of receivers to specific cores,
    /// allowing fine-grained control over receiver distribution and load balancing
    /// across multiple workflow execution contexts.
    /// 
    /// Can only be applied once per class to maintain clear core targeting.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class RunOnCoresAttribute : Attribute
    {
        /// <summary>
        /// Gets the array of core names where this receiver should be active.
        /// Contains the namespace identifiers of cores that should load and execute
        /// this receiver instance during workflow initialization.
        /// </summary>
        public string[] Cores { get; }

        /// <summary>
        /// Initializes a new instance of the RunOnCoresAttribute class with the specified core names.
        /// Allows specification of one or more core namespaces where the attributed receiver should run.
        /// </summary>
        /// <param name="cores">The names of cores where this receiver should be active</param>
        public RunOnCoresAttribute(params string[] cores)
        {
            Cores = cores;
        }
    }
}
