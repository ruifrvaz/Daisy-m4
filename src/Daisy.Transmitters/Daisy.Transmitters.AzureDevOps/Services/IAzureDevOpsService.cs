using Daisy.Resources.Interfaces;
using System.Threading.Tasks;
using Daisy.Transmitters.AzureDevOps.Models;
using Daisy.Resources.Signals;

namespace Daisy.Transmitters.AzureDevOps.Services
{
    public interface IAzureDevOpsService : IDaisyService
    {
        Task<int> CreateWorkItemAsync(UserStory story);

        Task<string> CloseWorkItemAsync(int workItemId, Impulse impulse);
    }
}
