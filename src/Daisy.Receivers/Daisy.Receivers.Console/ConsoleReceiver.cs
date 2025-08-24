using Daisy.Resources.Abstracts;
using Daisy.Resources.Attributes;
using Daisy.Resources.Signals;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace Daisy.Receivers.Console
{
    [RunOnCores("Daisy.Workflows.Starter")]
    public class ConsoleInput : AExternalReceiver
    {
        private readonly IEnumerable<string> _runOnCores;

        public override IEnumerable<string> RunOnCores => _runOnCores;

        public ConsoleInput(IEnumerable<string> runOnCores)
        {
            _runOnCores = runOnCores;
        }

        public ConsoleInput() : this(GetAttributeCores()) { }

        private static IEnumerable<string> GetAttributeCores()
        {
            return typeof(ConsoleInput).GetCustomAttribute<RunOnCoresAttribute>()?.Cores ?? Array.Empty<string>();
        }

        public override Impulse Receive()
        {
            return ReceiveAsync().GetAwaiter().GetResult();
        }

        public override Task<Impulse> ReceiveAsync()
        {
            System.Console.WriteLine($"--------------------------------------------------------");
            System.Console.WriteLine($"Type \"start\" to list workflows. Type \"bye\" to terminate.");
            System.Console.WriteLine($"--------------------------------------------------------");
            string input = System.Console.ReadLine();

            var trimmedInput = input.Trim();

            var impulse = new Impulse
            {
                Input = trimmedInput,
            };

            return Task.FromResult(impulse);
        }
    }
}
