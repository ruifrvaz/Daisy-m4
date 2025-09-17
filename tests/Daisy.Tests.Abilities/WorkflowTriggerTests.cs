using System.Threading.Tasks;
using Daisy.Abilities.Start;
using Daisy.Abilities.Start.Rules;
using Daisy.Resources.Extensions;
using Daisy.Resources.Models;
using Daisy.Resources.Pools;
using Daisy.Resources.Signals;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Daisy.Resources.Interfaces;
using System;

namespace Daisy.Tests.Abilities.Operator
{
    [TestClass]
    public class WorkflowTriggerTests
    {
        [TestInitialize]
        public void Initialize()
        {
            Cores.Instance.Pool.Clear();
        }

        [TestCleanup]
        public void Cleanup()
        {
            Cores.Instance.Pool.Clear();
        }

        [TestMethod]
        public void WorkflowTriggerCanTraverse_returns_true_for_weather_porto_input()
        {
            var rule = new WorkflowTriggerCanTraverse(new ApplicationSettings());
            var impulse = new Impulse { Input = "Weather: Porto" };

            rule.RuleApplies(impulse).Should().BeTrue();
        }

        [TestMethod]
        public void WorkflowTriggerCanTraverse_returns_false_for_other_input()
        {
            var rule = new WorkflowTriggerCanTraverse(new ApplicationSettings());
            var impulse = new Impulse { Input = "Hello World" };

            rule.RuleApplies(impulse).Should().BeFalse();
        }

        [TestMethod]
        public void WorkflowTriggerCanTraverse_returns_true_for_weather_porto_case_insensitive()
        {
            var rule = new WorkflowTriggerCanTraverse(new ApplicationSettings());
            var impulse = new Impulse { Input = "weather: porto" };

            rule.RuleApplies(impulse).Should().BeTrue();
        }

        [TestMethod]
        public void WorkflowTriggerCanTraverse_returns_true_for_weather_porto_mixed_case()
        {
            var rule = new WorkflowTriggerCanTraverse(new ApplicationSettings());
            var impulse = new Impulse { Input = "WeAtHeR: PoRtO" };

            rule.RuleApplies(impulse).Should().BeTrue();
        }

        [TestMethod]
        public async Task WorkflowTriggerPath_Traverse_adds_cityName_to_input_chain()
        {
            var path = new WorkflowTriggerPath(
                serviceProvider: null,
                traverseRules: new List<ITraverseRule>(),
                hasBeenTraversedRules: new List<ITraverseRule>(),
                pathName: "WorkflowTriggerPath",
                traverseOrder: 1,
                settings: new ApplicationSettings()
            );

            var impulse = new Impulse { Input = "Weather: Porto" };

            await path.Traverse(impulse);

            impulse.GetChainByKey("cityName").Should().Be("Porto");
        }

        [TestMethod]
        public async Task WorkflowTriggerPath_Traverse_adds_output_message()
        {
            var path = new WorkflowTriggerPath(
                serviceProvider: null,
                traverseRules: new List<ITraverseRule>(),
                hasBeenTraversedRules: new List<ITraverseRule>(),
                pathName: "WorkflowTriggerPath",
                traverseOrder: 1,
                settings: new ApplicationSettings()
            );

            var impulse = new Impulse { Input = "Weather: Porto" };

            await path.Traverse(impulse);

            impulse.Output.Should().Contain("WorkflowTriggerPath: Triggered workflow for Porto");
        }

        [TestMethod]
        public async Task WorkflowTriggerPath_Traverse_handles_different_cities()
        {
            var path = new WorkflowTriggerPath(
                serviceProvider: null,
                traverseRules: new List<ITraverseRule>(),
                hasBeenTraversedRules: new List<ITraverseRule>(),
                pathName: "WorkflowTriggerPath",
                traverseOrder: 1,
                settings: new ApplicationSettings()
            );

            var impulse = new Impulse { Input = "Weather: Lisbon" };

            await path.Traverse(impulse);

            impulse.GetChainByKey("cityName").Should().Be("Lisbon");
            impulse.Output.Should().Contain("WorkflowTriggerPath: Triggered workflow for Lisbon");
        }

        [TestMethod]
        public async Task WorkflowTriggerPath_Traverse_handles_city_with_spaces()
        {
            var path = new WorkflowTriggerPath(
                serviceProvider: null,
                traverseRules: new List<ITraverseRule>(),
                hasBeenTraversedRules: new List<ITraverseRule>(),
                pathName: "WorkflowTriggerPath",
                traverseOrder: 1,
                settings: new ApplicationSettings()
            );

            var impulse = new Impulse { Input = "Weather: New York" };

            await path.Traverse(impulse);

            impulse.GetChainByKey("cityName").Should().Be("New York");
            impulse.Output.Should().Contain("WorkflowTriggerPath: Triggered workflow for New York");
        }
    }
}