using Daisy.Receivers.WeatherEvent;
using Daisy.Resources.Signals;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Daisy.Tests.Receivers
{
    [TestClass]
    public class WeatherEventReceiverTest
    {
        [TestMethod]
        public async Task ReceiveAsync_returns_enqueued_impulse()
        {
            var receiver = new WeatherEventReceiver(new List<string>());
            var impulse = new Impulse { Input = "Lisbon" };

            receiver.RaiseEvent(impulse);
            var received = await receiver.ReceiveAsync();

            received.Input.Should().Be("Lisbon");
        }
    }
}
