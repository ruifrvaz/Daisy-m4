using Daisy.Resources.Abstracts;
using Daisy.Resources.Attributes;
using Daisy.Resources.Signals;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
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
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("-----------------------------------------------------------");
            stringBuilder.AppendLine("------------------------- Daisy-m4 ------------------------");
            stringBuilder.AppendLine("-----------------------------------------------------------");
            stringBuilder.AppendLine("\"start\" to list workflows.");
            stringBuilder.AppendLine("\"bye\" to terminate.");
            stringBuilder.AppendLine("Workflow name to start workflow (append args when required)");
            stringBuilder.AppendLine("-----------------------------------------------------------");
            System.Console.Write(stringBuilder.ToString());
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
