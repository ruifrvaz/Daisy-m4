using Daisy.Resources.Interfaces;
using System.Threading.Tasks;

namespace Daisy.Abilities.CopilotSdk.Services
{
    /// <summary>
    /// Service interface for GitHub Copilot SDK integration.
    /// Provides AI-powered agentic capabilities through the Copilot SDK.
    /// </summary>
    public interface ICopilotSdkService : IDaisyService
    {
        /// <summary>
        /// Sends a prompt to the Copilot SDK and waits for a complete response.
        /// </summary>
        /// <param name="prompt">The prompt to send to Copilot</param>
        /// <returns>The AI-generated response as a string</returns>
        Task<string> SendPromptAsync(string prompt);

        /// <summary>
        /// Checks if the Copilot SDK is available and properly configured.
        /// </summary>
        /// <returns>True if the SDK is available, false otherwise</returns>
        bool IsAvailable();
    }
}
