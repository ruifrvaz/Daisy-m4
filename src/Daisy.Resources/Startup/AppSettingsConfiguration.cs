using Daisy.Resources.Interfaces;
using Daisy.Resources.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.IO;

namespace Daisy.Resources.Startup
{
    public class AppSettingsConfiguration : IStartup
    {
        public IConfigurationRoot GetConfiguration()
        {
            var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appconfig.json", optional: false, reloadOnChange: true);

            builder.AddUserSecrets<ApplicationSettings>();

            return builder.Build();
        }

        public ApplicationSettings LoadApplicationSettings()
        {
            var settings = new ApplicationSettings();
            var configuration = GetConfiguration();
            
            configuration.GetSection("ApplicationSettings").Bind(settings);
            return settings;
        }

        public void SaveApplicationSettings(ApplicationSettings settings)
        {
            // Serialize the updated object back to JSON
            var updatedJson = JsonConvert.SerializeObject(new Root { ApplicationSettings = settings }, Formatting.Indented);

            var appSettingsPath = $"..\\..\\..\\appconfig.json";

            // update project assembly
            File.WriteAllText(appSettingsPath, updatedJson);

            // updated deployed assembly
            File.WriteAllText($"{Directory.GetCurrentDirectory()}\\appconfig.json", updatedJson);
        }
    }
}