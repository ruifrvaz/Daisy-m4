using Daisy.Abilities.OutputValidator.Rules;
using Daisy.Resources.Models;
using Daisy.Resources.Signals;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Daisy.Tests.Abilities.OutputValidator
{
    [TestClass]
    public class OutputValidatorRulesTests
    {
        [TestMethod]
        public void CanTraverse_rule_applies_when_output_is_empty()
        {
            var rule = new OutputValidatorCanTraverse(new ApplicationSettings());
            var impulse = new Impulse { Output = "" };

            var result = rule.RuleApplies(impulse);

            result.Should().BeTrue();
        }

        [TestMethod]
        public void CanTraverse_rule_applies_when_output_is_null()
        {
            var rule = new OutputValidatorCanTraverse(new ApplicationSettings());
            var impulse = new Impulse { Output = null };

            var result = rule.RuleApplies(impulse);

            result.Should().BeTrue();
        }

        [TestMethod]
        public void CanTraverse_rule_does_not_apply_when_output_has_value()
        {
            var rule = new OutputValidatorCanTraverse(new ApplicationSettings());
            var impulse = new Impulse { Output = "Some output" };

            var result = rule.RuleApplies(impulse);

            result.Should().BeFalse();
        }

        [TestMethod]
        public void Traversed_rule_applies_when_output_has_value()
        {
            var rule = new OutputValidatorTraversed(new ApplicationSettings());
            var impulse = new Impulse { Output = "Some output" };

            var result = rule.RuleApplies(impulse);

            result.Should().BeTrue();
        }

        [TestMethod]
        public void Traversed_rule_does_not_apply_when_output_is_empty()
        {
            var rule = new OutputValidatorTraversed(new ApplicationSettings());
            var impulse = new Impulse { Output = "" };

            var result = rule.RuleApplies(impulse);

            result.Should().BeFalse();
        }

        [TestMethod]
        public void Traversed_rule_does_not_apply_when_output_is_null()
        {
            var rule = new OutputValidatorTraversed(new ApplicationSettings());
            var impulse = new Impulse { Output = null };

            var result = rule.RuleApplies(impulse);

            result.Should().BeFalse();
        }
    }
}
