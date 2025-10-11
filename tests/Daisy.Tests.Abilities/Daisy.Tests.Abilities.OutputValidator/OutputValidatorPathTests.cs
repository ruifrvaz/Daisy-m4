using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Daisy.Abilities.OutputValidator.Paths;
using Daisy.Resources.Interfaces;
using Daisy.Resources.Models;
using Daisy.Resources.Pools;
using Daisy.Resources.Signals;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Daisy.Tests.Abilities.OutputValidator
{
    [TestClass]
    public class OutputValidatorPathTests
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
        public async Task Traverse_sets_default_message_when_output_is_empty()
        {
            var path = CreatePath();
            var impulse = new Impulse { Output = "" };

            await path.Traverse(impulse);

            impulse.Output.Should().Be("I don't know what you mean.");
        }

        [TestMethod]
        public async Task Traverse_sets_default_message_when_output_is_null()
        {
            var path = CreatePath();
            var impulse = new Impulse { Output = null };

            await path.Traverse(impulse);

            impulse.Output.Should().Be("I don't know what you mean.");
        }

        [TestMethod]
        public async Task Traverse_preserves_existing_output()
        {
            var path = CreatePath();
            var impulse = new Impulse { Output = "Existing output" };

            await path.Traverse(impulse);

            impulse.Output.Should().Be("Existing output");
        }

        private static OutputValidatorPath CreatePath()
        {
            return new OutputValidatorPath(
                new StubServiceProvider(),
                Array.Empty<ITraverseRule>(),
                Array.Empty<ITraverseRule>(),
                nameof(OutputValidatorPath),
                0,
                new ApplicationSettings());
        }

        private class StubServiceProvider : IServiceProvider
        {
            public object GetService(Type serviceType)
            {
                return null!;
            }
        }
    }
}
