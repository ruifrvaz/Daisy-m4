using Daisy.Resources.Interfaces;
using System.Threading.Tasks;

namespace Daisy.Abilities.Flights.Services
{
    public interface IFlightsService : IDaisyService
    {
        public Task<string> GetFlightsAsync(string city);
    }
}