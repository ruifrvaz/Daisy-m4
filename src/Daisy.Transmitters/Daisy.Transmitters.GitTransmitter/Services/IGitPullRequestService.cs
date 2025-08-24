using Daisy.Resources.Interfaces;
using Daisy.Transmitters.GitTransmitter.Models;

namespace Daisy.Transmitters.GitTransmitter.Services
{
    public interface IGitPullRequestService : IDaisyService
    {
        Task<int> CommitFilesAndCreatePullRequestAsync(UserStory story, IEnumerable<CoderFileMetadata> files, string repoZipPath);
    }
}
