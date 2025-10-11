using Daisy.Abilities.Weather.Rules;
using Daisy.Resources.Extensions;
using Daisy.Resources.Models;
using Daisy.Resources.Signals;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Daisy.Tests.Abilities.Weather
{
    [TestClass]
    public class GetWeatherByCityRulesTests
    {
        [TestMethod]
        public void CanTraverse_rule_applies_when_weather_chain_exists()
        {
            var rule = new GetWeatherByCityCanTraverse(new ApplicationSettings());
            var impulse = new Impulse();
            impulse.AddChain("weather: London");

            var result = rule.RuleApplies(impulse);

            result.Should().BeTrue();
        }

        [TestMethod]
        public void CanTraverse_rule_does_not_apply_when_weather_chain_missing()
        {
            var rule = new GetWeatherByCityCanTraverse(new ApplicationSettings());
            var impulse = new Impulse();

            var result = rule.RuleApplies(impulse);

            result.Should().BeFalse();
        }

        [TestMethod]
        public void Traversed_rule_applies_when_weather_in_output()
        {
            var rule = new Daisy.Abilities.Terminate.Rules.GetWeatherByCityTraversed(new ApplicationSettings());
            var impulse = new Impulse();
            impulse.AddChain("weather: London", ImpulseExtensions.ImpulseField.Output);

            var result = rule.RuleApplies(impulse);

            result.Should().BeTrue();
        }

        [TestMethod]
        public void Traversed_rule_does_not_apply_when_no_weather_in_output()
        {
            var rule = new Daisy.Abilities.Terminate.Rules.GetWeatherByCityTraversed(new ApplicationSettings());
            var impulse = new Impulse();

            var result = rule.RuleApplies(impulse);

            result.Should().BeFalse();
        }
    }
}
