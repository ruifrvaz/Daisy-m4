using System.Threading;
using System.Threading.Tasks;
using Daisy.Abilities.Terminate.Rules;
using Daisy.Resources.Interfaces;
using Daisy.Resources.Models;
using Daisy.Resources.Pools;
using Daisy.Resources.Signals;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Daisy.Tests.Abilities.Terminate
{
    [TestClass]
    public class TerminateRulesTests
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
        public void TerminateCanTraverse_returns_true_for_input_starting_with_bye()
        {
            var rule = new TerminateCanTraverse(new ApplicationSettings());
            var impulse = new Impulse { Input = "Bye Daisy" };

            rule.RuleApplies(impulse).Should().BeTrue();
        }

        [TestMethod]
        public void TerminateCanTraverse_returns_false_for_input_not_starting_with_bye()
        {
            var rule = new TerminateCanTraverse(new ApplicationSettings());
            var impulse = new Impulse { Input = "hello" };

            rule.RuleApplies(impulse).Should().BeFalse();
        }

        [TestMethod]
        public void TerminateTraversed_returns_true_when_all_cores_are_inactive()
        {
            Cores.Instance.Pool.Add(new TestCore { IsActive = false });
            Cores.Instance.Pool.Add(new TestCore { IsActive = false });

            var rule = new TerminateTraversed(new ApplicationSettings());

            rule.RuleApplies(new Impulse()).Should().BeTrue();
        }

        [TestMethod]
        public void TerminateTraversed_returns_false_when_any_core_is_active()
        {
            Cores.Instance.Pool.Add(new TestCore { IsActive = true });
            Cores.Instance.Pool.Add(new TestCore { IsActive = false });

            var rule = new TerminateTraversed(new ApplicationSettings());

            rule.RuleApplies(new Impulse()).Should().BeFalse();
        }

        private class TestCore : ICore
        {
            public bool IsActive { get; set; }

            public Task Start(CancellationToken token)
            {
                return Task.CompletedTask;
            }

            public void Stop()
            {
                IsActive = false;
            }
        }
    }
}
