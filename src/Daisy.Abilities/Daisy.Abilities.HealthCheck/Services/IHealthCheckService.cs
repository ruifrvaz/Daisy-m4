using Daisy.Resources.Interfaces;
using System.Threading.Tasks;

namespace Daisy.Abilities.HealthCheck.Services
{
    public interface IHealthCheckService : IDaisyService
    {
        Task<bool> CheckHealthAsync(string url);
    }
}
