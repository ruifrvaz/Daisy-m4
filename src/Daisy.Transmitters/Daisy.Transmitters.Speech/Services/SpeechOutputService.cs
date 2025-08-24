using Daisy.Receivers.Speech.Models;
using Daisy.Resources.Models;
using Microsoft.CognitiveServices.Speech;

namespace Daisy.Transmitters.Speech.Services
{
    public class SpeechOutputService : ISpeechOutputService
    {
        // Flag to track whether Dispose has been called
        private bool _disposed = false;

        private ApplicationSettings _settings;
        private SpeechOutputSettings _speechSettings;

        private SpeechSynthesizer _speechSynthesizer;

        public SpeechOutputService(ApplicationSettings settings)
        {
            _settings = settings;
            _speechSettings = settings.GetApiSettings<SpeechOutputSettings>("CognitiveServicesSpeech");
        }

        public void Initialize(IServiceProvider serviceProvider)
        {
            var speechConfig = SpeechConfig.FromSubscription(_speechSettings.SpeechKey, _speechSettings.SpeechRegion);
            speechConfig.SpeechSynthesisVoiceName = _speechSettings.SpeechSynthesisVoiceName;

            _speechSynthesizer = new SpeechSynthesizer(speechConfig);
        }

        public async Task<string> Speak(string text)
        {
            var result = string.Empty;
            using (var speechResult = await _speechSynthesizer.SpeakTextAsync(text))
            {
                if (speechResult.Reason == ResultReason.Canceled)
                {
                    var cancellation = SpeechSynthesisCancellationDetails.FromResult(speechResult);
                    Console.WriteLine($"CANCELED: Reason={cancellation.Reason}");

                    if (cancellation.Reason == CancellationReason.Error)
                    {
                        Console.WriteLine($"CANCELED: ErrorCode={cancellation.ErrorCode}");
                        Console.WriteLine($"CANCELED: ErrorDetails=[{cancellation.ErrorDetails}]");
                        Console.WriteLine($"CANCELED: Did you update the subscription info?");
                    }
                }
                result = speechResult.Reason.ToString();
            }

            return result;
        }

        // Dispose method implementing IDisposable interface
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // Dispose method to release resources
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _speechSynthesizer.Dispose();
                }

                // Dispose unmanaged resources here
                // For example, releasing a handle or closing a file

                _disposed = true;
            }
        }

        // Destructor (finalizer) to ensure resources are released even if Dispose() is not called
        ~SpeechOutputService()
        {
            Dispose(false);
        }
    }
}
