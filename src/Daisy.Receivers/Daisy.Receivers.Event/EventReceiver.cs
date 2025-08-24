using Daisy.Resources.Abstracts;
using Daisy.Resources.Signals;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Daisy.Receivers.Event
{
    public class EventInput : AEventReceiver
    {
        public override IEnumerable<string> RunOnCores => new[] { "Daisy.Workflows.Default" };

        private static readonly ConcurrentQueue<Impulse> _inputQueue = new();

        private static readonly SemaphoreSlim _signal = new(0);

        public override Impulse Receive()
        {
            return ReceiveAsync().GetAwaiter().GetResult();
        }

        public async override Task<Impulse> ReceiveAsync()
        {
            // Wait until input is received
            await _signal.WaitAsync();

            if (_inputQueue.TryDequeue(out var input))
            {
                return input;
            }

            return new Impulse
            {
                Input = string.Empty,
                Output = string.Empty,
                Error = "No input was available after signal."
            };
        }

        public override void RaiseEvent(Impulse input)
        {
            _inputQueue.Enqueue(input);
            _signal.Release();
        }

    }
}
