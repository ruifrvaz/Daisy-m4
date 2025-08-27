using Daisy.Abilities.Assistant.Operator.Models;
using Daisy.Resources.Interfaces;
using System.Threading.Tasks;

namespace Daisy.Abilities.Assistant.Operator.Services
{
    public interface IAssistantService : IDaisyService
    {
        Task<string> CreateThreadAsync();

        Task<string> AddMessageToThreadAsync(string message, string role, string threadId);

        Task<string> RunAssistantOnThreadAsync(string assistantId, string threadId);

        Task<string> CheckRunStatusAsync(string threadId, string runId);

        Task<ThreadConversation> DisplayMessagesAsync(string threadId);

        string DetermineFunction(string assistantMessage);

        Task<RunDetails> GetRunDetailsAsync(string threadId, string runId);

        Task SubmitToolOutputsAsync(string threadId, string runId, ToolOutput[] toolOutputs);
    }
}
