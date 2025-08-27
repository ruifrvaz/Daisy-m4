using Daisy.Resources.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daisy.Transmitters.Speech.Services
{
    public interface ISpeechOutputService : IDaisyService, IDisposable
    {
        public Task<string> Speak(string text);
    }
}
