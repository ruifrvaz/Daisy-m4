using Daisy.Abilities.Assistant.Coder.Models;
using Daisy.Resources.Interfaces;
using System.Threading.Tasks;

namespace Daisy.Abilities.Assistant.Coder.Services
{
    public interface IAssistantService : IDaisyService
    {
        public Task<string> CreateThreadAsync();

        public Task<string> RunAssistantOnThreadAsync(string runInstructions, string assistantId, string threadId);

        public Task<string> AddMessageToThreadAsync(string threadMessage, string role, string threadId);

        public Task<ThreadConversation> DisplayMessagesAsync(string threadId);

        public Task<string> CheckRunStatusAsync(string threadId, string runId);
    }
}
