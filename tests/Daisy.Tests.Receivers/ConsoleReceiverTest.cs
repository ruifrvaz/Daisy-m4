using Daisy.Receivers.Console;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Daisy.Tests.Receivers
{
    [TestClass]
    public class ConsoleReceiverTest
    {
        [TestMethod]
        public async Task ReceiveAsync_trims_input()
        {
            var originalIn = Console.In;
            try
            {
                Console.SetIn(new StringReader("  trimmed  "));
                var receiver = new ConsoleReceiver(new[] { "TestCore" });
                var impulse = await receiver.ReceiveAsync();
                impulse.Input.Should().Be("trimmed");
            }
            finally
            {
                Console.SetIn(originalIn);
            }
        }

        [TestMethod]
        public async Task ReceiveAsync_handles_null_input()
        {
            var originalIn = Console.In;
            try
            {
                // Simulate scenario where Console.ReadLine() returns null (e.g., stdin closed)
                Console.SetIn(new StringReader(string.Empty));
                var receiver = new ConsoleReceiver(new[] { "TestCore" });
                var impulse = await receiver.ReceiveAsync();
                impulse.Input.Should().Be(string.Empty);
            }
            finally
            {
                Console.SetIn(originalIn);
            }
        }
    }
}
