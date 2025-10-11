using System;
using System.Collections.Generic;
using Daisy.Abilities.Flights.Paths;
using Daisy.Abilities.Flights.Rules;
using Daisy.Resources.Extensions;
using Daisy.Resources.Interfaces;
using Daisy.Resources.Models;
using Daisy.Resources.Pools;
using Daisy.Resources.Signals;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Daisy.Tests.Abilities.Flights
{
    [TestClass]
    public class GetFlightsByCityPathTests
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
        public void CanTraverse_rule_applies_when_flights_chain_exists()
        {
            var rule = new GetFlightsByCityCanTraverse(new ApplicationSettings());
            var impulse = new Impulse();
            impulse.AddChain("flights: Paris");

            var result = rule.RuleApplies(impulse);

            result.Should().BeTrue();
        }

        [TestMethod]
        public void CanTraverse_rule_does_not_apply_when_flights_chain_missing()
        {
            var rule = new GetFlightsByCityCanTraverse(new ApplicationSettings());
            var impulse = new Impulse();

            var result = rule.RuleApplies(impulse);

            result.Should().BeFalse();
        }

        [TestMethod]
        public void Traversed_rule_applies_when_flights_in_output()
        {
            var rule = new GetFlightsByCityTraversed(new ApplicationSettings());
            var impulse = new Impulse();
            impulse.AddChain("flights: Paris", ImpulseExtensions.ImpulseField.Output);

            var result = rule.RuleApplies(impulse);

            result.Should().BeTrue();
        }

        [TestMethod]
        public void Traversed_rule_does_not_apply_when_no_flights_in_output()
        {
            var rule = new GetFlightsByCityTraversed(new ApplicationSettings());
            var impulse = new Impulse();

            var result = rule.RuleApplies(impulse);

            result.Should().BeFalse();
        }
    }
}
