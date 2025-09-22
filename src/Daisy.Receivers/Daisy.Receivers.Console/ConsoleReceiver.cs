using Daisy.Resources.Abstracts;
using Daisy.Resources.Signals;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Daisy.Receivers.Console
{
    public class ConsoleReceiver : AExternalReceiver
    {
        private readonly IEnumerable<string> _runOnCores;

        public override IEnumerable<string> RunOnCores => _runOnCores;

        public ConsoleReceiver(IEnumerable<string> runOnCores)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("------------------------------------------------------------");
            stringBuilder.AppendLine("------------------------- Daisy-m4 -------------------------");
            stringBuilder.AppendLine("------------------------------------------------------------");
            stringBuilder.AppendLine("Options:");
            stringBuilder.AppendLine("\"start\" to list workflows.");
            stringBuilder.AppendLine("\"bye\" to terminate.");
            stringBuilder.AppendLine("{WorkflowName} to start workflow (append args when required)");
            stringBuilder.AppendLine("------------------------------------------------------------");
            System.Console.Write(stringBuilder.ToString());

            _runOnCores = runOnCores;
        }

        public override Impulse Receive()
        {
            return ReceiveAsync().GetAwaiter().GetResult();
        }

        public override Task<Impulse> ReceiveAsync()
        {
            string input = System.Console.ReadLine();

            var trimmedInput = input?.Trim() ?? string.Empty;

            var impulse = new Impulse
            {
                Input = trimmedInput,
            };

            return Task.FromResult(impulse);
        }
    }
}
