using Daisy.Resources.Abstracts;
using Daisy.Resources.Signals;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Daisy.Receivers.FlightsEvent
{
    /// <summary>
    /// Event-based receiver for the Flights workflow.
    /// Waits for a city name event raised by the Starter workflow and
    /// forwards the impulse to the Flights workflow.
    /// </summary>
    public class FlightsEventReceiver : AEventReceiver
    {
        /// <summary>
        /// Only runs on the Flights workflow core.
        /// </summary>
        private readonly IEnumerable<string> _runOnCores;

        public override IEnumerable<string> RunOnCores => _runOnCores;

        private static readonly ConcurrentQueue<Impulse> _inputQueue = new();
        private static readonly SemaphoreSlim _signal = new(0);


        /// <summary>
        /// Standard constructor that will be used during assembly injection
        /// </summary>
        /// <param name="runOnCores"></param>
        public FlightsEventReceiver(IEnumerable<string> runOnCores)
        {
            _runOnCores = runOnCores;
        }

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