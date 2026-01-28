using Daisy.Abilities.CopilotSdk.Rules;
using Daisy.Resources.Extensions;
using Daisy.Resources.Models;
using Daisy.Resources.Signals;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Daisy.Tests.Abilities.CopilotSdk
{
    [TestClass]
    public class CopilotSdkRulesTests
    {
        [TestMethod]
        public void CanTraverse_rule_applies_when_copilot_chain_exists()
        {
            // Arrange
            var rule = new CopilotSdkCanTraverse(new ApplicationSettings());
            var impulse = new Impulse();
            impulse.AddChain("copilot: Write a hello world program in C#");

            // Act
            var result = rule.RuleApplies(impulse);

            // Assert
            result.Should().BeTrue();
        }

        [TestMethod]
        public void CanTraverse_rule_does_not_apply_when_copilot_chain_missing()
        {
            // Arrange
            var rule = new CopilotSdkCanTraverse(new ApplicationSettings());
            var impulse = new Impulse();

            // Act
            var result = rule.RuleApplies(impulse);

            // Assert
            result.Should().BeFalse();
        }

        [TestMethod]
        public void CanTraverse_rule_does_not_apply_when_copilot_chain_is_empty()
        {
            // Arrange
            var rule = new CopilotSdkCanTraverse(new ApplicationSettings());
            var impulse = new Impulse();
            impulse.AddChain("copilot: ");

            // Act
            var result = rule.RuleApplies(impulse);

            // Assert
            result.Should().BeFalse();
        }

        [TestMethod]
        public void Traversed_rule_applies_when_copilot_in_output()
        {
            // Arrange
            var rule = new CopilotSdkTraversed(new ApplicationSettings());
            var impulse = new Impulse();
            impulse.AddChain("Copilot: Here is a hello world program...", ImpulseExtensions.ImpulseField.Output);

            // Act
            var result = rule.RuleApplies(impulse);

            // Assert
            result.Should().BeTrue();
        }

        [TestMethod]
        public void Traversed_rule_does_not_apply_when_no_copilot_in_output()
        {
            // Arrange
            var rule = new CopilotSdkTraversed(new ApplicationSettings());
            var impulse = new Impulse();

            // Act
            var result = rule.RuleApplies(impulse);

            // Assert
            result.Should().BeFalse();
        }

        [TestMethod]
        public void Traversed_rule_does_not_apply_when_copilot_in_input_only()
        {
            // Arrange
            var rule = new CopilotSdkTraversed(new ApplicationSettings());
            var impulse = new Impulse();
            impulse.AddChain("copilot: Write a program");

            // Act
            var result = rule.RuleApplies(impulse);

            // Assert
            result.Should().BeFalse();
        }
    }
}
