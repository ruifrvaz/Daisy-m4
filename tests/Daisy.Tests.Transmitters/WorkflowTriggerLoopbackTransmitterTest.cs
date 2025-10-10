using Daisy.Abilities.Operator.Paths;
using Daisy.Resources.Extensions;
using Daisy.Resources.Interfaces;
using Daisy.Resources.Pools;
using Daisy.Resources.Signals;
using Daisy.Transmitters.WorkflowTrigger;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace Daisy.Tests.Transmitters
{
    [TestClass]
    public class WorkflowTriggerLoopbackTransmitterTest
    {
        [TestInitialize]
        public void TestInitialize()
        {
            // Clear the event receivers pool before each test
            EventReceivers.Instance.Pool.Clear();
        }

        [TestMethod]
        public void CanTransmit_returns_false_for_null_impulse()
        {
            var transmitter = new WorkflowTriggerLoopbackTransmitter();

            var result = transmitter.CanTransmit(null!);

            result.Should().BeFalse();
        }

        [TestMethod]
        public void CanTransmit_returns_false_when_impulse_is_not_loopback()
        {
            var transmitter = new WorkflowTriggerLoopbackTransmitter();
            var impulse = new Impulse { IsLoopback = false };
            impulse.AddChain("workflowIdentifier: Weather");

            var result = transmitter.CanTransmit(impulse);

            result.Should().BeFalse();
        }

        [TestMethod]
        public void CanTransmit_returns_false_when_workflow_identifier_is_missing()
        {
            var transmitter = new WorkflowTriggerLoopbackTransmitter();
            var impulse = new Impulse { IsLoopback = true };

            var result = transmitter.CanTransmit(impulse);

            result.Should().BeFalse();
        }

        [TestMethod]
        public void CanTransmit_returns_false_when_workflow_identifier_is_empty()
        {
            var transmitter = new WorkflowTriggerLoopbackTransmitter();
            var impulse = new Impulse { IsLoopback = true };
            impulse.AddChain("workflowIdentifier:");

            var result = transmitter.CanTransmit(impulse);

            result.Should().BeFalse();
        }

        [TestMethod]
        public void CanTransmit_returns_false_when_workflow_identifier_is_whitespace()
        {
            var transmitter = new WorkflowTriggerLoopbackTransmitter();
            var impulse = new Impulse { IsLoopback = true };
            impulse.AddChain("workflowIdentifier:   ");

            var result = transmitter.CanTransmit(impulse);

            result.Should().BeFalse();
        }

        [TestMethod]
        public void CanTransmit_returns_true_when_impulse_is_loopback_and_has_workflow_identifier()
        {
            var transmitter = new WorkflowTriggerLoopbackTransmitter();
            var impulse = new Impulse { IsLoopback = true };
            impulse.AddChain("workflowIdentifier: Weather");

            var result = transmitter.CanTransmit(impulse);

            result.Should().BeTrue();
        }

        [TestMethod]
        public void TransmitLoopBack_does_nothing_when_cannot_transmit()
        {
            var transmitter = new WorkflowTriggerLoopbackTransmitter();
            var impulse = new Impulse { IsLoopback = false };

            // Should not throw
            transmitter.TransmitLoopBack(impulse);
        }

        [TestMethod]
        public void TransmitLoopBack_does_nothing_when_workflow_identifier_becomes_empty_after_extraction()
        {
            var transmitter = new WorkflowTriggerLoopbackTransmitter();
            var impulse = new Impulse { IsLoopback = true };
            impulse.AddChain("workflowIdentifier:");

            // Should not throw
            transmitter.TransmitLoopBack(impulse);
        }

        [TestMethod]
        public void TransmitLoopBack_raises_event_on_all_event_receivers()
        {
            // Arrange
            var mockReceiver1 = new MockEventReceiver();
            var mockReceiver2 = new MockEventReceiver();
            EventReceivers.Instance.Pool.Add(mockReceiver1);
            EventReceivers.Instance.Pool.Add(mockReceiver2);

            var transmitter = new WorkflowTriggerLoopbackTransmitter();
            var impulse = new Impulse { IsLoopback = true };
            impulse.AddChain("workflowIdentifier: Weather");
            impulse.AddChain("Weather: London");

            // Act
            transmitter.TransmitLoopBack(impulse);

            // Assert
            mockReceiver1.RaisedEvents.Should().HaveCount(1);
            mockReceiver2.RaisedEvents.Should().HaveCount(1);

            var raisedImpulse = mockReceiver1.RaisedEvents.First();
            raisedImpulse.GetChainByKey("Weather").Should().Be("London");
        }

        [TestMethod]
        public void TransmitLoopBack_builds_workflow_invocation_chain_without_parameters()
        {
            // Arrange
            var mockReceiver = new MockEventReceiver();
            EventReceivers.Instance.Pool.Add(mockReceiver);

            var transmitter = new WorkflowTriggerLoopbackTransmitter();
            var impulse = new Impulse { IsLoopback = true };
            impulse.AddChain("workflowIdentifier: Weather");

            // Act
            transmitter.TransmitLoopBack(impulse);

            // Assert
            mockReceiver.RaisedEvents.Should().HaveCount(1);
            var raisedImpulse = mockReceiver.RaisedEvents.First();
            raisedImpulse.GetChainByKey("Weather").Should().Be("");
        }

        [TestMethod]
        public void TransmitLoopBack_builds_workflow_invocation_chain_with_parameters()
        {
            // Arrange
            var mockReceiver = new MockEventReceiver();
            EventReceivers.Instance.Pool.Add(mockReceiver);

            var transmitter = new WorkflowTriggerLoopbackTransmitter();
            var impulse = new Impulse { IsLoopback = true };
            impulse.AddChain("workflowIdentifier: Weather");
            impulse.AddChain("Weather: London");

            // Act
            transmitter.TransmitLoopBack(impulse);

            // Assert
            mockReceiver.RaisedEvents.Should().HaveCount(1);
            var raisedImpulse = mockReceiver.RaisedEvents.First();
            raisedImpulse.GetChainByKey("Weather").Should().Be("London");
        }

        [TestMethod]
        public void TransmitLoopBack_handles_empty_workflow_parameters()
        {
            // Arrange
            var mockReceiver = new MockEventReceiver();
            EventReceivers.Instance.Pool.Add(mockReceiver);

            var transmitter = new WorkflowTriggerLoopbackTransmitter();
            var impulse = new Impulse { IsLoopback = true };
            impulse.AddChain("workflowIdentifier: Weather");
            impulse.AddChain("Weather:");

            // Act
            transmitter.TransmitLoopBack(impulse);

            // Assert
            mockReceiver.RaisedEvents.Should().HaveCount(1);
            var raisedImpulse = mockReceiver.RaisedEvents.First();
            raisedImpulse.GetChainByKey("Weather").Should().Be("");
        }

        [TestMethod]
        public void TransmitLoopBack_does_not_raise_events_when_no_receivers()
        {
            var transmitter = new WorkflowTriggerLoopbackTransmitter();
            var impulse = new Impulse { IsLoopback = true };
            impulse.AddChain("workflowIdentifier: Weather");

            // Should not throw when no receivers
            transmitter.TransmitLoopBack(impulse);
        }
    }

    // Mock event receiver for testing
    public class MockEventReceiver : IEventReceiver
    {
        public List<Impulse> RaisedEvents { get; } = new List<Impulse>();

        public bool IsActive { get; set; }

        public IEnumerable<string> RunOnCores => new List<string> { "Daisy.Workflows.Weather" };

        public void RaiseEvent(Impulse input)
        {
            RaisedEvents.Add(input);
        }

        public Impulse Receive()
        {
            throw new System.NotImplementedException();
        }

        public System.Threading.Tasks.Task<Impulse> ReceiveAsync()
        {
            throw new System.NotImplementedException();
        }

        public System.Threading.Tasks.Task Start(System.Threading.CancellationToken token)
        {
            IsActive = true;
            return System.Threading.Tasks.Task.CompletedTask;
        }

        public void Stop()
        {
            IsActive = false;
        }
    }
}