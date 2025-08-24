using Daisy.Resources.Interfaces;
using System.Threading.Tasks;
using Daisy.Receivers.AzureDevopsReceiver.Models;

namespace Daisy.Receivers.AzureDevopsReceiver.Services
{
    public interface IPullRequestApprovalService : IDaisyService
    {
        Task<string> GetPullRequestStatusAsync(UserStory story, int pullRequestId);
    }
}
