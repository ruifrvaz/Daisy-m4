using Daisy.Receivers.TryMeEvent;
using Daisy.Resources.Signals;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace Daisy.Tests.Receivers
{
    [TestClass]
    public class TryMeEventReceiverTest
    {
        [TestMethod]
        public async Task ReceiveAsync_returns_enqueued_impulse()
        {
            var receiver = new TryMeEventReceiver();
            var impulse = new Impulse { Input = "Lisbon" };

            receiver.RaiseEvent(impulse);
            var received = await receiver.ReceiveAsync();

            received.Input.Should().Be("Lisbon");
        }
    }
}
