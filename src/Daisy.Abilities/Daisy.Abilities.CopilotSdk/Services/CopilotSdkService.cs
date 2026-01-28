using Daisy.Abilities.CopilotSdk.Models;
using Daisy.Resources.Models;
using GitHub.Copilot.SDK;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Daisy.Abilities.CopilotSdk.Services
{
    /// <summary>
    /// Service implementation for GitHub Copilot SDK integration.
    /// Manages Copilot client lifecycle and provides AI capabilities to Daisy workflows.
    /// </summary>
    public class CopilotSdkService : ICopilotSdkService
    {
        private readonly CopilotSdkSettings _settings;
        private CopilotClient _client;
        private bool _isInitialized;

        public CopilotSdkService(ApplicationSettings settings)
        {
            _settings = settings.GetApiSettings<CopilotSdkSettings>("CopilotSdk") ?? new CopilotSdkSettings();
        }

        public void Initialize(IServiceProvider serviceProvider)
        {
            try
            {
                // Initialize the Copilot client
                // The SDK will automatically manage the Copilot CLI process
                _client = new CopilotClient();
                _isInitialized = true;
            }
            catch (Exception ex)
            {
                // If Copilot CLI is not installed or not authenticated, gracefully handle it
                Console.WriteLine($"Warning: Unable to initialize Copilot SDK: {ex.Message}");
                _isInitialized = false;
            }
        }

        public bool IsAvailable()
        {
            return _isInitialized && _client != null;
        }

        public async Task<string> SendPromptAsync(string prompt)
        {
            if (!IsAvailable())
            {
                return "Copilot SDK is not available. Please ensure Copilot CLI is installed and authenticated.";
            }

            try
            {
                // Create a new session with configured settings
                await using var session = await _client.CreateSessionAsync(new SessionConfig
                {
                    Model = _settings.Model,
                    Streaming = _settings.Streaming,
                });

                // For streaming responses, collect the content
                var responseBuilder = new StringBuilder();
                var completionSource = new TaskCompletionSource<string>();

                // Handle streaming events
                session.On(ev =>
                {
                    if (ev is AssistantMessageDeltaEvent deltaEvent)
                    {
                        responseBuilder.Append(deltaEvent.Data.DeltaContent);
                    }
                    else if (ev is SessionIdleEvent)
                    {
                        // Session is idle, response is complete
                        completionSource.TrySetResult(responseBuilder.ToString());
                    }
                    else if (ev is SessionErrorEvent errorEvent)
                    {
                        // Error occurred
                        completionSource.TrySetException(
                            new Exception($"Copilot SDK error: {errorEvent.Data.Message}")
                        );
                    }
                });

                // Send the prompt
                await session.SendAsync(new MessageOptions { Prompt = prompt }, CancellationToken.None);

                // Wait for the response with timeout
                var timeoutTask = Task.Delay(TimeSpan.FromSeconds(_settings.TimeoutSeconds));
                var completedTask = await Task.WhenAny(completionSource.Task, timeoutTask);

                if (completedTask == timeoutTask)
                {
                    return "Request timed out waiting for Copilot response.";
                }

                return await completionSource.Task;
            }
            catch (Exception ex)
            {
                return $"Error communicating with Copilot SDK: {ex.Message}";
            }
        }

        public void Dispose()
        {
            _client?.DisposeAsync().AsTask().Wait();
            _client = null;
            _isInitialized = false;
        }
    }
}
