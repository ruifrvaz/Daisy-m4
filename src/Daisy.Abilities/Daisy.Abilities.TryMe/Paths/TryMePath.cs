using Daisy.Resources.Abstracts;
using Daisy.Resources.Extensions;
using Daisy.Resources.Interfaces;
using Daisy.Resources.Models;
using Daisy.Resources.Signals;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Daisy.Abilities.TryMe
{
    /// <summary>
    /// Ability path that fetches weather information for a given city using a public API.
    /// </summary>
    public class TryMePath : APath
    {
        public TryMePath(IServiceProvider serviceProvider,
            IEnumerable<ITraverseRule> traverseRules,
            IEnumerable<ITraverseRule> hasBeenTraversedRules,
            string pathName,
            int traverseOrder,
            ApplicationSettings settings)
            : base(serviceProvider, traverseRules, hasBeenTraversedRules, pathName, traverseOrder, settings) { }

        /// <summary>
        /// Performs an HTTP GET request to retrieve weather data for the city contained in the impulse input.
        /// Adds the result to the impulse output or emits an error message if the request fails.
        /// </summary>
        public override async Task Traverse(Impulse impulse)
        {
            var city = impulse.Input.FirstLetterUpperCase();
            try
            {
                var factory = ServiceProvider.GetService<IHttpClientFactory>();
                var client = factory?.CreateClient("DefaultClient") ?? new HttpClient();
                var url = $"https://wttr.in/{Uri.EscapeDataString(city)}?format=j1";
                using var response = await client.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    impulse.Output = impulse.Output.AddPrefix($"TryMePath: Unable to retrieve weather for {city}.");
                    await Emit(impulse);
                    return;
                }

                using var json = await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync());
                if (json.RootElement.TryGetProperty("current_condition", out var conditionArray) && conditionArray.GetArrayLength() > 0)
                {
                    var condition = conditionArray[0];
                    var temp = condition.GetProperty("temp_C").GetString();
                    var desc = condition.GetProperty("weatherDesc")[0].GetProperty("value").GetString();
                    impulse.Output = impulse.Output.AddPrefix($"TryMePath: The weather in {city} is {desc} at {temp}°C.");
                }
                else
                {
                    impulse.Output = impulse.Output.AddPrefix($"TryMePath: Unable to parse weather for {city}.");
                }
            }
            catch (Exception)
            {
                impulse.Output = impulse.Output.AddPrefix($"TryMePath: Error retrieving weather for {city}.");
            }

            await Emit(impulse);
        }
    }
}
