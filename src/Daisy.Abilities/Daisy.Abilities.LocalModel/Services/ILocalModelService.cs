using Daisy.Resources.Interfaces;
using System;
using System.Threading.Tasks;

namespace Daisy.Abilities.LocalModel.Services
{
    public interface ILocalModelService : IDaisyService
    {
        Task<string> GetResponseAsync(string prompt);
    }
}
