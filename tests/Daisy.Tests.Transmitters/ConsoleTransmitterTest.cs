using Daisy.Resources.Signals;
using Daisy.Transmitters.Console;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace Daisy.Tests.Transmitters
{
    [TestClass]
    public class ConsoleTransmitterTest
    {
        [TestMethod]
        public void CanTransmit_always_returns_true()
        {
            var transmitter = new ConsoleTransmitter();
            var impulse = new Impulse { Output = "test output" };

            var result = transmitter.CanTransmit(impulse);

            result.Should().BeTrue();
        }

        [TestMethod]
        public void CanTransmit_returns_true_for_null_impulse()
        {
            var transmitter = new ConsoleTransmitter();

            var result = transmitter.CanTransmit(null!);

            result.Should().BeTrue();
        }

        [TestMethod]
        public void Transmit_writes_output_to_console()
        {
            var originalOut = Console.Out;
            try
            {
                using var stringWriter = new StringWriter();
                Console.SetOut(stringWriter);

                var transmitter = new ConsoleTransmitter();
                var impulse = new Impulse { Output = "Hello World" };

                transmitter.Transmit(impulse);

                var output = stringWriter.ToString();
                output.Should().Contain("Hello World");
            }
            finally
            {
                Console.SetOut(originalOut);
            }
        }

        [TestMethod]
        public void Transmit_writes_error_to_console_when_present()
        {
            var originalOut = Console.Out;
            try
            {
                using var stringWriter = new StringWriter();
                Console.SetOut(stringWriter);

                var transmitter = new ConsoleTransmitter();
                var impulse = new Impulse
                {
                    Output = "Hello World",
                    Error = "  Error occurred  "
                };

                transmitter.Transmit(impulse);

                var output = stringWriter.ToString();
                output.Should().Contain("Hello World");
                output.Should().Contain("Error occurred");
                output.Should().NotContain("  Error occurred  "); // Should be trimmed
            }
            finally
            {
                Console.SetOut(originalOut);
            }
        }

        [TestMethod]
        public void Transmit_does_not_write_error_when_null()
        {
            var originalOut = Console.Out;
            try
            {
                using var stringWriter = new StringWriter();
                Console.SetOut(stringWriter);

                var transmitter = new ConsoleTransmitter();
                var impulse = new Impulse
                {
                    Output = "Hello World",
                    Error = null
                };

                transmitter.Transmit(impulse);

                var output = stringWriter.ToString();
                output.Should().Contain("Hello World");
                output.Should().NotContain("Error");
            }
            finally
            {
                Console.SetOut(originalOut);
            }
        }

        [TestMethod]
        public void Transmit_does_not_write_error_when_empty()
        {
            var originalOut = Console.Out;
            try
            {
                using var stringWriter = new StringWriter();
                Console.SetOut(stringWriter);

                var transmitter = new ConsoleTransmitter();
                var impulse = new Impulse
                {
                    Output = "Hello World",
                    Error = ""
                };

                transmitter.Transmit(impulse);

                var output = stringWriter.ToString();
                output.Should().Contain("Hello World");
                output.Should().NotContain("Error");
            }
            finally
            {
                Console.SetOut(originalOut);
            }
        }

        [TestMethod]
        public void Transmit_does_not_write_error_when_whitespace()
        {
            var originalOut = Console.Out;
            try
            {
                using var stringWriter = new StringWriter();
                Console.SetOut(stringWriter);

                var transmitter = new ConsoleTransmitter();
                var impulse = new Impulse
                {
                    Output = "Hello World",
                    Error = "   "
                };

                transmitter.Transmit(impulse);

                var output = stringWriter.ToString();
                output.Should().Contain("Hello World");
                output.Should().NotContain("Error");
            }
            finally
            {
                Console.SetOut(originalOut);
            }
        }

        [TestMethod]
        public void Transmit_handles_null_output()
        {
            var originalOut = Console.Out;
            try
            {
                using var stringWriter = new StringWriter();
                Console.SetOut(stringWriter);

                var transmitter = new ConsoleTransmitter();
                var impulse = new Impulse { Output = null };

                transmitter.Transmit(impulse);

                var output = stringWriter.ToString();
                output.Should().NotBeNull(); // null gets written as empty
            }
            finally
            {
                Console.SetOut(originalOut);
            }
        }
    }
}