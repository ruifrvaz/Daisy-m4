using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daisy.Receivers.Speech.Models
{
    internal class SpeechOutputSettings
    {
        public string SpeechKey { get; set; }

        public string SpeechRegion { get; set; }
        
        public string SpeechSynthesisVoiceName { get; set; }
    }
}
