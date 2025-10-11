using System;
using System.Collections.Generic;
using Daisy.Abilities.LocalModel.Paths;
using Daisy.Abilities.LocalModel.Rules;
using Daisy.Resources.Extensions;
using Daisy.Resources.Interfaces;
using Daisy.Resources.Models;
using Daisy.Resources.Pools;
using Daisy.Resources.Signals;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Daisy.Tests.Abilities.LocalModel
{
    [TestClass]
    public class GetLocalModelResponsePathTests
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
        public void CanTraverse_rule_applies_when_localmodel_chain_exists()
        {
            var rule = new GetLocalModelResponseCanTraverse(new ApplicationSettings());
            var impulse = new Impulse();
            impulse.AddChain("localmodel: Hello, how are you?");

            var result = rule.RuleApplies(impulse);

            result.Should().BeTrue();
        }

        [TestMethod]
        public void CanTraverse_rule_does_not_apply_when_localmodel_chain_missing()
        {
            var rule = new GetLocalModelResponseCanTraverse(new ApplicationSettings());
            var impulse = new Impulse();

            var result = rule.RuleApplies(impulse);

            result.Should().BeFalse();
        }

        [TestMethod]
        public void Traversed_rule_applies_when_localmodel_in_output()
        {
            var rule = new GetLocalModelResponseTraversed(new ApplicationSettings());
            var impulse = new Impulse();
            impulse.AddChain("localmodel: Response", ImpulseExtensions.ImpulseField.Output);

            var result = rule.RuleApplies(impulse);

            result.Should().BeTrue();
        }

        [TestMethod]
        public void Traversed_rule_does_not_apply_when_no_localmodel_in_output()
        {
            var rule = new GetLocalModelResponseTraversed(new ApplicationSettings());
            var impulse = new Impulse();

            var result = rule.RuleApplies(impulse);

            result.Should().BeFalse();
        }
    }
}
