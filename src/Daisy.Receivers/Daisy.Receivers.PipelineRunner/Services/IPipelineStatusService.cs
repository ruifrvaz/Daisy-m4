using Daisy.Resources.Interfaces;
using System.Threading.Tasks;
using Daisy.Receivers.PipelineRunner.Models;

namespace Daisy.Receivers.PipelineRunner.Services
{
    public interface IPipelineStatusService : IDaisyService
    {
        Task<string> GetPipelineRunStatusAsync(string pipelineId, string runId);
    }
}
