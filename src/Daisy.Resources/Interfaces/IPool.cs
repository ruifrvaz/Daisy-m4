using System.Collections.Generic;

namespace Daisy.Resources.Interfaces
{
    /// <summary>
    /// Defines the contract for pool collections that manage module instances in the Daisy engine.
    /// Pools are singleton collections that enable module reuse across cores and support 
    /// inter-core communication within the workflow orchestration system.
    /// </summary>
    /// <typeparam name="T">The type of objects managed by this pool</typeparam>
    public interface IPool<T>
    {
        /// <summary>
        /// Gets or sets the collection of pooled instances.
        /// This list contains all active instances of the specified type
        /// that are available for use across the workflow engine.
        /// </summary>
        List<T> Pool { get; set; }
    }
}