using Daisy.Resources.Interfaces;
using System;
using System.Threading.Tasks;

namespace Daisy.Abilities.ImageToText.Services
{
    internal interface IOcrService : IDaisyService, IDisposable
    {
        Task<string> ReadText(string imagePath);
    }
}
