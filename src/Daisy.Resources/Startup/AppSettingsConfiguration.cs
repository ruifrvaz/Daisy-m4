using Daisy.Resources.Interfaces;
using Daisy.Resources.Models;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Text.Json;
using System.Xml;

namespace Daisy.Resources.Startup
{
    public class AppSettingsConfiguration : IStartup
    {
        public IConfigurationRoot GetConfiguration()
        {
            var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appconfig.json", optional: false, reloadOnChange: true);

            builder.AddUserSecrets(typeof(AppSettingsConfiguration).Assembly);

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
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            var updatedJson = JsonSerializer.Serialize(new Root { ApplicationSettings = settings }, options);

            var appSettingsPath = $"..\\..\\..\\appconfig.json";

            // update project assembly
            File.WriteAllText(appSettingsPath, updatedJson);

            // updated deployed assembly
            File.WriteAllText($"{Directory.GetCurrentDirectory()}\\appconfig.json", updatedJson);
        }
    }
}