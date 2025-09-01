using Daisy.Resources.Extensions;
using Daisy.Resources.Signals;
using FluentAssertions;

namespace Daisy.Tests.Extensions
{
    [TestClass]
    public class ImpulseExtensionsTest
    {
        [TestMethod]
        public void AddChain_AppendsChainToInput()
        {
            var impulse = new Impulse();
            impulse.AddChain("cityName: London", ImpulseField.Input);
            impulse.Input.Should().Be("<cityName: London>");
        }

        [TestMethod]
        public void AddChain_AppendsChainToOutput()
        {
            var impulse = new Impulse();
            impulse.AddChain("weather: Sunny", ImpulseField.Output);
            impulse.Output.Should().Be("<weather: Sunny>");
        }

        [TestMethod]
        public void GetChainByKey_ReturnsValue_FromInput()
        {
            var impulse = new Impulse();
            impulse.AddChain("cityName: Paris", ImpulseField.Input);
            var city = impulse.GetChainByKey("cityName", ImpulseField.Input);
            city.Should().Be("Paris");
        }

        [TestMethod]
        public void GetChainByKey_ReturnsValue_FromOutput()
        {
            var impulse = new Impulse();
            impulse.AddChain("weather: Rainy", ImpulseField.Output);
            var weather = impulse.GetChainByKey("weather", ImpulseField.Output);
            weather.Should().Be("Rainy");
        }
    }
}
