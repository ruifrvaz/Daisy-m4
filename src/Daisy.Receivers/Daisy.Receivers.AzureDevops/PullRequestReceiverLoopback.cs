using Daisy.Receivers.AzureDevopsReceiver.Models;
using Daisy.Receivers.AzureDevopsReceiver.Services;
using Daisy.Resources.Abstracts;
using Daisy.Resources.Extensions;
using Daisy.Resources.Services;
using Daisy.Resources.Signals;

namespace Daisy.Receivers.AzureDevopsReceiver
{
    public class PullRequestReceiverLoopback : ALoopBackReceiver
    {
        private readonly IPullRequestApprovalService _service;

        public PullRequestReceiverLoopback()
        {
            _service = ServiceContainer.Instance.GetService<IPullRequestApprovalService>() as IPullRequestApprovalService;
        }

        public override bool CanReceive(Impulse impulse)
        {
            return impulse.Input.StartsWith("PullRequestId:", StringComparison.InvariantCultureIgnoreCase);
        }

        public async override Task ReceiveLoopBack(Impulse impulse)
        {
            if (!CanReceive(impulse))
            {
                return;
            }

            var prIdString = impulse.Input.GetChainByKey("PullRequestId");
            var storyJson = impulse.Output.GetChainByKey("StoryParsed");
            string status = string.Empty;

            if (int.TryParse(prIdString, out int prId) && !string.IsNullOrWhiteSpace(storyJson))
            {
                var story = storyJson.GetData<UserStory>();
                
                if (story != null)
                {
                    const int totalAttempts = 30;
                    for (int i = 0; i < totalAttempts; i++)
                    {
                        status = await _service.GetPullRequestStatusAsync(story, prId);
                        switch (status)
                        {
                            case "approved":
                                break;
                            case "abandoned":
                                break;
                            default:
                                System.Console.WriteLine($"    Awaiting approval, attempt {i + 1}/{totalAttempts}");
                                break;
                        }

                        if (status == "approved" || status == "abandoned" || status == "completed")
                        {
                            break; // Exit the for loop
                        }
                        await Task.Delay(TimeSpan.FromSeconds(10));
                    }
                }

                if (string.IsNullOrWhiteSpace(status))
                {
                    impulse.Error = "Pull request approval timed out.";
                }
                else
                {
                    impulse.Input = impulse.Input.AddPrefix($"PullRequestStatus: {status} > ");
                }
            }

            System.Console.WriteLine($"12. Pull request {status}.");
            await base.ReceiveLoopBack(impulse);
        }
    }
}
