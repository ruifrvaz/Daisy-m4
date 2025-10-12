using System;
using System.Collections.Generic;
using Daisy.Abilities.DatabaseStorage.Paths;
using Daisy.Abilities.DatabaseStorage.Rules;
using Daisy.Resources.Extensions;
using Daisy.Resources.Interfaces;
using Daisy.Resources.Models;
using Daisy.Resources.Pools;
using Daisy.Resources.Signals;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Daisy.Tests.Abilities.DatabaseStorage
{
    [TestClass]
    public class StoreFootballScoresPathTests
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
        public void CanTraverse_rule_applies_when_footballScores_in_output()
        {
            var rule = new StoreFootballScoresCanTraverse(new ApplicationSettings());
            var impulse = new Impulse();
            impulse.AddChain("footballScores: Mock scores", ImpulseExtensions.ImpulseField.Output);

            var result = rule.RuleApplies(impulse);

            result.Should().BeTrue();
        }

        [TestMethod]
        public void CanTraverse_rule_does_not_apply_when_footballScores_missing()
        {
            var rule = new StoreFootballScoresCanTraverse(new ApplicationSettings());
            var impulse = new Impulse();

            var result = rule.RuleApplies(impulse);

            result.Should().BeFalse();
        }

        [TestMethod]
        public void Traversed_rule_applies_when_stored_message_in_output()
        {
            var rule = new StoreFootballScoresTraversed(new ApplicationSettings());
            var impulse = new Impulse();
            impulse.AddChain("databaseStored: Scores stored for Manchester United", ImpulseExtensions.ImpulseField.Output);

            var result = rule.RuleApplies(impulse);

            result.Should().BeTrue();
        }

        [TestMethod]
        public void Traversed_rule_does_not_apply_when_no_stored_message()
        {
            var rule = new StoreFootballScoresTraversed(new ApplicationSettings());
            var impulse = new Impulse();

            var result = rule.RuleApplies(impulse);

            result.Should().BeFalse();
        }
    }
}
