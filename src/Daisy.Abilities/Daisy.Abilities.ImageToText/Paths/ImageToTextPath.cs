using Daisy.Abilities.ImageToText.Services;
using Daisy.Resources.Abstracts;
using Daisy.Resources.Extensions;
using Daisy.Resources.Interfaces;
using Daisy.Resources.Models;
using Daisy.Resources.Services;
using Daisy.Resources.Signals;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Daisy.Abilities.ImageToText.Paths
{
    public class ImageToTextPath : APath
    {
        private readonly IOcrService _ocrService;

        public ImageToTextPath(IServiceProvider serviceProvider,
            IEnumerable<ITraverseRule> traverseRules,
            IEnumerable<ITraverseRule> hasBeenTraversedRules,
            string pathName,
            int traverseOrder,
            ApplicationSettings settings)
            : base(serviceProvider, traverseRules, hasBeenTraversedRules, pathName, traverseOrder, settings)
        {
            _ocrService = ServiceContainer.Instance.GetService<IOcrService>() as IOcrService;
        }

        public override async Task Traverse(Impulse impulse)
        {
            var filePath = impulse.Input.GetChainByKey("FilePath");
            if (!File.Exists(filePath))
            {
                impulse.Error = "File not found.";
                await Emit(impulse);
            }
            else
            {
                var text = await _ocrService.ReadText(filePath);

                impulse.Output = impulse.Output.AddPrefix($"ImageConverted: {text.Trim()} >");
            }

            System.Console.WriteLine("2. Converted image to text.");
            await Emit(impulse);
        }
    }
}
