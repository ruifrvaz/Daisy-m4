using Azure;
using Azure.AI.Vision.ImageAnalysis;
using Daisy.Abilities.ImageToText.Models;
using Daisy.Resources.Models;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daisy.Abilities.ImageToText.Services
{
    public class OcrService : IOcrService
    {
        private bool _disposed;
        private readonly OcrSettings _ocrSettings;
        private ImageAnalysisClient _engine;

        public OcrService(ApplicationSettings settings)
        {
            _ocrSettings = settings.GetApiSettings<OcrSettings>("OcrService");
        }

        public void Initialize(IServiceProvider serviceProvider)
        {
            _engine = new ImageAnalysisClient(new Uri(_ocrSettings.Endpoint), new AzureKeyCredential(_ocrSettings.Key));
        }

        public Task<string> ReadText(string imagePath)
        {
            using FileStream stream = new FileStream(imagePath, FileMode.Open);

            ImageAnalysisResult result = _engine.Analyze(BinaryData.FromStream(stream), VisualFeatures.Read);

            var text = new StringBuilder();
            foreach (var line in result.Read.Blocks.SelectMany(block => block.Lines))
            {
                text.AppendLine(line.Text);
            }

            return Task.FromResult(text.ToString());
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // nothing to dispose
                }
                _disposed = true;
            }
        }

        ~OcrService()
        {
            Dispose(false);
        }
    }
}
