using Daisy.Abilities.Assistant.Coder.Models;
using Daisy.Resources.Interfaces;
using System.Collections.Generic;

namespace Daisy.Abilities.Assistant.Coder.Services
{
    public interface ICodeParserService : IDaisyService
    {
        public string ParseCoderFiles(string assistantMessage);
    }
}
