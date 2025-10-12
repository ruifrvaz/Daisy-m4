using System;
using System.Collections.Generic;
using Daisy.Abilities.Football.Paths;
using Daisy.Abilities.Football.Rules;
using Daisy.Resources.Extensions;
using Daisy.Resources.Interfaces;
using Daisy.Resources.Models;
using Daisy.Resources.Pools;
using Daisy.Resources.Signals;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Daisy.Tests.Abilities.Football
{
    [TestClass]
    public class GetFootballScoresPathTests
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
        public void CanTraverse_rule_applies_when_football_chain_exists()
        {
            var rule = new GetFootballScoresCanTraverse(new ApplicationSettings());
            var impulse = new Impulse();
            impulse.AddChain("football: Manchester United");

            var result = rule.RuleApplies(impulse);

            result.Should().BeTrue();
        }

        [TestMethod]
        public void CanTraverse_rule_does_not_apply_when_football_chain_missing()
        {
            var rule = new GetFootballScoresCanTraverse(new ApplicationSettings());
            var impulse = new Impulse();

            var result = rule.RuleApplies(impulse);

            result.Should().BeFalse();
        }

        [TestMethod]
        public void Traversed_rule_applies_when_footballScores_in_output()
        {
            var rule = new GetFootballScoresTraversed(new ApplicationSettings());
            var impulse = new Impulse();
            impulse.AddChain("footballScores: Mock scores", ImpulseExtensions.ImpulseField.Output);

            var result = rule.RuleApplies(impulse);

            result.Should().BeTrue();
        }

        [TestMethod]
        public void Traversed_rule_does_not_apply_when_no_footballScores_in_output()
        {
            var rule = new GetFootballScoresTraversed(new ApplicationSettings());
            var impulse = new Impulse();

            var result = rule.RuleApplies(impulse);

            result.Should().BeFalse();
        }
    }
}
