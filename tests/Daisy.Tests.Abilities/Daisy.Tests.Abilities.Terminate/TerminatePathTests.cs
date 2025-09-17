using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Daisy.Abilities.Terminate;
using Daisy.Resources.Interfaces;
using Daisy.Resources.Models;
using Daisy.Resources.Pools;
using Daisy.Resources.Signals;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Daisy.Tests.Abilities.Terminate
{
    [TestClass]
    public class TerminatePathTests
    {
        [TestInitialize]
        public void Initialize()
        {
            Cores.Instance.Pool.Clear();
            EventReceivers.Instance.Pool.Clear();
            Paths.Instance.Pool.Clear();
            LoopBackTransmitters.Instance.Pool.Clear();
            ExternalTransmitters.Instance.Pool.Clear();
        }

        [TestMethod]
        public async Task Traverse_should_stop_active_cores_and_raise_event()
        {
            var activeCore = new TestCore { IsActive = true };
            var inactiveCore = new TestCore { IsActive = false };
            Cores.Instance.Pool.Add(activeCore);
            Cores.Instance.Pool.Add(inactiveCore);

            var eventReceiver = new TestEventReceiver();
            EventReceivers.Instance.Pool.Add(eventReceiver);

            var impulse = new Impulse();
            var path = CreatePath();

            await path.Traverse(impulse);

            activeCore.StopCalled.Should().BeTrue();
            inactiveCore.StopCalled.Should().BeFalse();
            eventReceiver.ReceivedImpulses.Should().ContainSingle();
            eventReceiver.ReceivedImpulses.Single().Output.Should().Be("Workflow terminating.");
            impulse.Output.Should().Be("Goodbye.");
        }

        private static TerminatePath CreatePath()
        {
            return new TerminatePath(
                new StubServiceProvider(),
                Array.Empty<ITraverseRule>(),
                Array.Empty<ITraverseRule>(),
                nameof(TerminatePath),
                0,
                new ApplicationSettings());
        }

        private class StubServiceProvider : IServiceProvider
        {
            public object GetService(Type serviceType)
            {
                return null!;
            }
        }

        private class TestCore : ICore
        {
            public bool StopCalled { get; private set; }
            public bool IsActive { get; set; }

            public Task Start(CancellationToken token)
            {
                return Task.CompletedTask;
            }

            public void Stop()
            {
                StopCalled = true;
                IsActive = false;
            }
        }

        private class TestEventReceiver : IEventReceiver
        {
            private readonly List<Impulse> _receivedImpulses = new List<Impulse>();

            public IReadOnlyCollection<Impulse> ReceivedImpulses => _receivedImpulses;

            public bool IsActive { get; set; }

            public IEnumerable<string> RunOnCores => Enumerable.Empty<string>();

            public Task<Impulse> ReceiveAsync()
            {
                return Task.FromResult(new Impulse());
            }

            public Impulse Receive()
            {
                return new Impulse();
            }

            public Task Start(CancellationToken token)
            {
                return Task.CompletedTask;
            }

            public void Stop()
            {
            }

            public void RaiseEvent(Impulse input)
            {
                _receivedImpulses.Add(input);
            }
        }
    }
}
