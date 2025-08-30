using Daisy.Resources.Interfaces;
using System.Threading.Tasks;

namespace Daisy.Abilities.GitAssistant.Services
{
    public interface IGitService : IDaisyService
    {
        Task<bool> EnsureRepositoryExistsAsync(string repositoryName, string branch);
        
        Task<string> FetchLatestAsync(string repoUrl, string repositoryName, string branch);
    }
}
