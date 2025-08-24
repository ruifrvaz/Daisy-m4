using Daisy.Abilities.Assistant.StoryParser.Models;
using Daisy.Resources.Interfaces;
using System.Threading.Tasks;

namespace Daisy.Abilities.Assistant.StoryParser.Services
{
    public interface IAssistantService : IDaisyService
    {
        Task<string> CreateThreadAsync();
        Task<string> AddMessageToThreadAsync(string message, string role, string threadId);
        Task<string> RunAssistantOnThreadAsync(string assistantId, string threadId);
        Task<string> CheckRunStatusAsync(string threadId, string runId);
        Task<ThreadConversation> DisplayMessagesAsync(string threadId);
    }
}
