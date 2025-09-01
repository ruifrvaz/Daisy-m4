using Daisy.Resources.Abstracts;
using Daisy.Resources.Signals;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Daisy.Receivers.TryMeEvent
{
    /// <summary>
    /// Event-based receiver for the TryMe workflow.
    /// Waits for a city name event raised by the Starter workflow and
    /// forwards the impulse to the TryMe workflow.
    /// </summary>
    public class TryMeEventInput : AEventReceiver
    {
        /// <summary>
        /// Only runs on the TryMe workflow core.
        /// </summary>
        public override IEnumerable<string> RunOnCores => new[] { "Daisy.Workflows.TryMe" };

        private static readonly ConcurrentQueue<Impulse> _inputQueue = new();
        private static readonly SemaphoreSlim _signal = new(0);

        /// <summary>
        /// Synchronously receives an impulse when available.
        /// </summary>
        public override Impulse Receive()
        {
            return ReceiveAsync().GetAwaiter().GetResult();
        }

        /// <summary>
        /// Waits for a city name event and returns the queued impulse.
        /// </summary>
        public async override Task<Impulse> ReceiveAsync()
        {
            await _signal.WaitAsync();

            if (_inputQueue.TryDequeue(out var impulse))
            {
                return impulse;
            }

            return new Impulse
            {
                Input = string.Empty,
                Output = string.Empty,
                Error = "No input was available after signal."
            };
        }

        /// <summary>
        /// Enqueues an incoming event and signals waiting receivers.
        /// </summary>
        public override void RaiseEvent(Impulse input)
        {
            _inputQueue.Enqueue(input);
            _signal.Release();
        }
    }
}
