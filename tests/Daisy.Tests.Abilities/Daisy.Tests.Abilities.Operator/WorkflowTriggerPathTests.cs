using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Daisy.Abilities.Operator.Paths;
using Daisy.Abilities.Operator.Rules;
using Daisy.Resources.Extensions;
using Daisy.Resources.Interfaces;
using Daisy.Resources.Models;
using Daisy.Resources.Pools;
using Daisy.Resources.Services;
using Daisy.Resources.Signals;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Daisy.Tests.Abilities.Operator
{
    [TestClass]
    public class WorkflowTriggerPathTests
    {
        [TestInitialize]
        public void Initialize()
        {
            Cores.Instance.Pool = new List<ICore>();
            LoopBackTransmitters.Instance.Pool = new List<ILoopBackTransmitter>();
            ExternalTransmitters.Instance.Pool = new List<IExternalTransmitter>();
            Paths.Instance.Pool = new List<IPath>();

            // Clear and initialize ServiceContainer with PathFinderService
            ServiceContainer.Instance.Services.Clear();
            ServiceContainer.Instance.Services.Add(new PathFinderService(new ApplicationSettings()));
        }

        [TestMethod]
        public void RuleApplies_ReturnsTrue_WhenInputMatchesAvailableWorkflow()
        {
            Cores.Instance.Pool.Add(new Daisy.Workflows.Weather.FakeWeatherCore());
            var settings = new ApplicationSettings();
            var rule = new WorkflowTriggerCanTraverse(settings);
            var impulse = new Impulse
            {
                Input = "Weather: Porto"
            };

            var applies = rule.RuleApplies(impulse);

            applies.Should().BeTrue();
        }

        [TestMethod]
        public async Task Traverse_PreparesLoopbackImpulseForWorkflowTrigger()
        {
            Cores.Instance.Pool.Add(new Daisy.Workflows.Weather.FakeWeatherCore());
            var path = new WorkflowTriggerPath(
                new NoopServiceProvider(),
                Enumerable.Empty<ITraverseRule>(),
                Enumerable.Empty<ITraverseRule>(),
                "WorkflowTrigger",
                0,
                new ApplicationSettings());
            var impulse = new Impulse
            {
                Input = "Weather: Porto"
            };

            await path.Traverse(impulse);

            impulse.IsLoopback.Should().BeTrue();
            impulse.Input.Should().Be("<workflowIdentifier: Weather><Weather: Porto>");
            impulse.GetChainByKey("workflowIdentifier").Should().Be("Weather");
            impulse.GetChainByKey("Weather").Should().Be("Porto");
            impulse.Output.Should().Be("Triggering Weather workflow with \"Porto\".");
        }

        private sealed class NoopServiceProvider : IServiceProvider
        {
            public object? GetService(Type serviceType)
            {
                return null;
            }
        }
    }
}

namespace Daisy.Workflows.Weather
{
    internal sealed class FakeWeatherCore : ICore
    {
        public bool IsActive { get; set; }

        public Task Start(CancellationToken token)
        {
            return Task.CompletedTask;
        }

        public void Stop()
        {
        }
    }
}
