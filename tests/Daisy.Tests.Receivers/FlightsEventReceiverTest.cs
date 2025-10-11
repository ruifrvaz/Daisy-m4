using Daisy.Receivers.FlightsEvent;
using Daisy.Resources.Signals;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Daisy.Tests.Receivers
{
    [TestClass]
    public class FlightsEventReceiverTest
    {
        [TestMethod]
        public async Task ReceiveAsync_returns_enqueued_impulse()
        {
            var receiver = new FlightsEventReceiver(new List<string>());
            var impulse = new Impulse { Input = "Paris" };

            receiver.RaiseEvent(impulse);
            var received = await receiver.ReceiveAsync();

            received.Input.Should().Be("Paris");
        }
    }
}
