using Daisy.Resources.Interfaces;
using System.Threading.Tasks;

namespace Daisy.Abilities.DatabaseStorage.Services
{
    public interface IDatabaseService : IDaisyService
    {
        Task<bool> StoreFootballScoresAsync(string clubName, string scores);
    }
}
