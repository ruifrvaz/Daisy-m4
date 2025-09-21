using System.Threading;
using System.Threading.Tasks;

namespace Daisy.Resources.Interfaces
{
    /// <summary>
    /// Defines the base contract for task-based components in the Daisy orchestration engine.
    /// This interface provides the fundamental lifecycle management capabilities for all
    /// long-running components such as cores, receivers, and other workflow modules.
    /// </summary>
    public interface ITask
    {
        /// <summary>
        /// Gets or sets a value indicating whether this task is currently active and running.
        /// Used to track the operational state and control task lifecycle.
        /// </summary>
        bool IsActive { get; set; }

        /// <summary>
        /// Starts the task with the specified cancellation token.
        /// Initializes and begins the task's primary operations.
        /// </summary>
        /// <param name="token">The cancellation token to monitor for cancellation requests</param>
        /// <returns>A task representing the asynchronous start operation</returns>
        Task Start(CancellationToken token);

        /// <summary>
        /// Stops the task and cleanly shuts down any ongoing operations.
        /// Should ensure all resources are properly disposed and the task state is reset.
        /// </summary>
        void Stop();
    }
}