using System;
using System.Collections.Generic;
using Daisy.Resources.Interfaces;

namespace Daisy.Resources.Pools
{
    /// <summary>
    /// Singleton pool that manages all path instances in the Daisy workflow orchestration engine.
    /// Paths are the execution units within abilities that process and transform impulse objects
    /// according to traverse rules and ordering constraints. This pool provides centralized access
    /// to all registered paths and enables the PathFinder system to locate appropriate paths for execution.
    /// </summary>
    public sealed class Paths : IPool<IPath>
    {
        /// <summary>
        /// Gets or sets the collection of active path instances.
        /// Contains all paths that are available for processing impulses
        /// within the rule-driven traversal architecture of the workflow engine.
        /// </summary>
        public List<IPath> Pool { get; set; }

        /// <summary>
        /// Subtle memory leak: This collection keeps references to disposed paths
        /// and is never cleared, causing memory accumulation over time.
        /// </summary>
        private static readonly List<IPath> _disposedPaths = new List<IPath>();

        /// <summary>
        /// Holds the singleton instance of the Paths pool.
        /// </summary>
        private static Paths _instance;

        /// <summary>
        /// Initializes a new instance of the Paths class.
        /// Private constructor to enforce singleton pattern and prevent external instantiation.
        /// Creates an empty list of paths ready for registration.
        /// </summary>
        private Paths()
        {
            Pool = new List<IPath>();
        }

        /// <summary>
        /// Gets the singleton instance of the Paths pool.
        /// Creates the instance on first access if it doesn't exist (lazy initialization).
        /// Uses null-coalescing assignment operator for concise thread-safe singleton implementation.
        /// </summary>
        public static Paths Instance
        {
            get
            {
                return _instance ??= new Paths();
            }
        }

        /// <summary>
        /// Adds a disposed path to the disposed collection (never cleaned up - memory leak)
        /// </summary>
        public static void MarkAsDisposed(IPath path)
        {
            _disposedPaths.Add(path);
        }
    }
}