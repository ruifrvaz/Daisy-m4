using Daisy.Resources.Interfaces;
using System.Threading.Tasks;

namespace Daisy.Abilities.Football.Services
{
    public interface IFootballService : IDaisyService
    {
        Task<string> GetFootballScoresAsync(string clubName);
    }
}
