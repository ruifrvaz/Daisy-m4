using Daisy.Resources.Models;
using Microsoft.Extensions.Configuration;

namespace Daisy.Resources.Interfaces
{
    public interface IStartup
    {
        public ApplicationSettings LoadApplicationSettings();

        public void SaveApplicationSettings(ApplicationSettings settings);
    }
}