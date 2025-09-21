using System;
using System.Collections.Concurrent;
using Daisy.Resources.Signals;

namespace Daisy.Resources.Interfaces
{
    /// <summary>
    /// Defines the contract for workflow execution containers in the Daisy orchestration engine.
    /// Cores are the main execution threads that manage workflow lifecycles and coordinate
    /// between receivers, abilities, and transmitters. Each core can run in parallel with others
    /// and communicates through shared pools.
    /// </summary>
    public interface ICore : ITask
    {
    }
}