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
    public class CopilotSdkService : ICopilotSdkService, IAsyncDisposable
    {
        private readonly CopilotSdkSettings _settings;
        private readonly object _lock = new object();
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

            CopilotSession session = null;
            try
            {
                // Create a cancellation token with timeout
                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(_settings.TimeoutSeconds));

                // Create a new session with configured settings
                session = await _client.CreateSessionAsync(new SessionConfig
                {
                    Model = _settings.Model,
                    Streaming = _settings.Streaming,
                }, cts.Token);

                // For streaming responses, collect the content
                var responseBuilder = new StringBuilder();
                var completionSource = new TaskCompletionSource<string>();

                // Handle streaming events
                // Note: The SDK guarantees sequential event delivery, so no explicit synchronization needed for StringBuilder
                session.On(ev =>
                {
                    if (ev is AssistantMessageDeltaEvent deltaEvent)
                    {
                        lock (_lock)
                        {
                            responseBuilder.Append(deltaEvent.Data.DeltaContent);
                        }
                    }
                    else if (ev is SessionIdleEvent)
                    {
                        // Session is idle, response is complete
                        lock (_lock)
                        {
                            completionSource.TrySetResult(responseBuilder.ToString());
                        }
                    }
                    else if (ev is SessionErrorEvent errorEvent)
                    {
                        // Error occurred
                        completionSource.TrySetException(
                            new Exception($"Copilot SDK error: {errorEvent.Data.Message}")
                        );
                    }
                });

                // Send the prompt with cancellation token
                await session.SendAsync(new MessageOptions { Prompt = prompt }, cts.Token);

                // Wait for the response (timeout is handled by CancellationToken)
                return await completionSource.Task;
            }
            catch (OperationCanceledException)
            {
                return "Request timed out waiting for Copilot response.";
            }
            catch (Exception ex)
            {
                return $"Error communicating with Copilot SDK: {ex.Message}";
            }
            finally
            {
                // Properly dispose the session to free resources
                if (session != null)
                {
                    await session.DisposeAsync();
                }
            }
        }

        public void Dispose()
        {
            // For synchronous dispose, use async dispose helper
            DisposeAsync().AsTask().GetAwaiter().GetResult();
        }

        public async ValueTask DisposeAsync()
        {
            if (_client != null)
            {
                await _client.DisposeAsync();
                _client = null;
            }
            _isInitialized = false;
        }
    }
}
