using Daisy.Resources.Abstracts;
using Daisy.Resources.Signals;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Daisy.Receivers.Console
{
    public class ConsoleInput : AExternalReceiver
    {
        //TODO: improve loading. use attributes or apssettings configuration
        public override IEnumerable<string> RunOnCores => new[] { "Daisy.Workflows.Starter" };

        public override Impulse Receive()
        {
            return ReceiveAsync().GetAwaiter().GetResult();
        }

        public override Task<Impulse> ReceiveAsync()
        {
            string input = System.Console.ReadLine();

            var trimmedInput = string.Format(input).Trim(' ');

            var impulse = new Impulse
            {
                Input = trimmedInput,
            };

            return Task.FromResult(impulse);
        }
    }
}