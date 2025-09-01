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
            impulse.AddChain("cityName: London");
            impulse.Input.Should().Be("<cityName: London>");
        }

        [TestMethod]
        public void GetChainByKey_ReturnsValue()
        {
            var impulse = new Impulse();
            impulse.AddChain("cityName: Paris");
            var city = impulse.Input.GetChainByKey("cityName");
            city.Should().Be("Paris");
        }
    }
}
