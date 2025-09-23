#nullable enable
using Daisy.Resources.Interfaces;
using System.Threading.Tasks;

namespace Daisy.Abilities.Weather.Services
{
    public interface IWeatherService : IDaisyService
    {
        public Task<string?> GetWeatherAsync(string city);
    }
}
