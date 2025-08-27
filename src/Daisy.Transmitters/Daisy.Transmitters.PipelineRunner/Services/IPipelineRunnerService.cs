using Daisy.Resources.Interfaces;
using System.Threading.Tasks;
using Daisy.Transmitters.PipelineRunner.Models;

namespace Daisy.Transmitters.PipelineRunner.Services
{
    public interface IPipelineRunnerService : IDaisyService
    {
        Task<string> RunPipelineAsync(UserStory story);
    }
}
