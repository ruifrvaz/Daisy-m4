using Daisy.Resources.Extensions;
using Daisy.Resources.Signals;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Daisy.Tests.Extensions
{
    [TestClass]
    public class ImpulseExtensionsTest
    {
        [TestMethod]
        public void AddChain_AppendsChainToInput()
        {
            var impulse = new Impulse();
            impulse.AddChain("cityName: London");
            impulse.Input.Should().Be("<cityName: London>");
            impulse.Output.Should().BeEmpty();
        }

        [TestMethod]
        public void AddChain_AppendsChainToOutput()
        {
            var impulse = new Impulse();
            impulse.AddChain("cityName: London", ImpulseExtensions.ImpulseField.Output);
            impulse.Output.Should().Be("<cityName: London>");
            impulse.Input.Should().BeEmpty();
        }

        [TestMethod]
        public void GetChainByKey_FromInput_ReturnsValue()
        {
            var impulse = new Impulse();
            impulse.AddChain("cityName: Paris", ImpulseExtensions.ImpulseField.Input);
            var city = impulse.GetChainByKey("cityName", ImpulseExtensions.ImpulseField.Input);
            city.Should().Be("Paris");
        }

        [TestMethod]
        public void GetChainByKey_FromOutput_ReturnsValue()
        {
            var impulse = new Impulse();
            impulse.AddChain("cityName: Paris", ImpulseExtensions.ImpulseField.Output);
            var city = impulse.GetChainByKey("cityName", ImpulseExtensions.ImpulseField.Output);
            city.Should().Be("Paris");
        }

        [TestMethod]
        public void GetLastChain_FromOutput_ReturnsLastChain()
        {
            var impulse = new Impulse();
            impulse.AddChain("cityName: Paris", ImpulseExtensions.ImpulseField.Output);
            impulse.AddChain("cityName: London", ImpulseExtensions.ImpulseField.Output);

            var chain = impulse.GetLastChain("cityName", ImpulseExtensions.ImpulseField.Output);

            chain.Should().Be("cityName: London");
        }

        [TestMethod]
        public void GetLastChain_FromInput_ReturnsLastChain()
        {
            var impulse = new Impulse();
            impulse.AddChain("cityName: Paris", ImpulseExtensions.ImpulseField.Input);
            impulse.AddChain("cityName: Madrid", ImpulseExtensions.ImpulseField.Input);

            var chain = impulse.GetLastChain("cityName", ImpulseExtensions.ImpulseField.Input);

            chain.Should().Be("cityName: Madrid");
        }

        [TestMethod]
        public void GetLastChain_WithoutField_PrefersOutput()
        {
            var impulse = new Impulse();
            impulse.AddChain("cityName: Berlin", ImpulseExtensions.ImpulseField.Input);
            impulse.AddChain("cityName: Rome", ImpulseExtensions.ImpulseField.Output);

            var chain = impulse.GetLastChain("cityName");

            chain.Should().Be("cityName: Rome");
        }

        [TestMethod]
        public void GetLastChain_WhenKeyNotFound_ReturnsNull()
        {
            var impulse = new Impulse();
            impulse.AddChain("cityName: Lisbon", ImpulseExtensions.ImpulseField.Input);

            var chain = impulse.GetLastChain("countryName");

            chain.Should().BeNull();
        }
    }
}
