namespace Daisy.Abilities.CopilotSdk.Models
{
    /// <summary>
    /// Configuration settings for the GitHub Copilot SDK integration.
    /// </summary>
    public class CopilotSdkSettings
    {
        /// <summary>
        /// The AI model to use for Copilot sessions (e.g., "gpt-4.1", "gpt-4o").
        /// </summary>
        public string Model { get; set; } = "gpt-4.1";

        /// <summary>
        /// Whether to enable streaming responses for real-time feedback.
        /// </summary>
        public bool Streaming { get; set; } = true;

        /// <summary>
        /// Timeout in seconds for Copilot operations. Default is 30 seconds.
        /// </summary>
        public int TimeoutSeconds { get; set; } = 30;
    }
}
