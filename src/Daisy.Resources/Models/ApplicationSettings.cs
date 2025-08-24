using System.Collections.Generic;
using System.Text.Json;

namespace Daisy.Resources.Models
{
    public class Root
    {
        public ApplicationSettings ApplicationSettings { get; set; }
    }

    public class ApplicationSettings
    {
        public string SolutionName { get; set; }

        public List<string> Receivers { get; set; } = new List<string>();

        public List<string> Transmitters { get; set; } = new List<string>();

        public List<string> Abilities { get; set; } = new List<string>();

        public List<string> Workflows { get; set; } = new List<string>();

        public Dictionary<string, Dictionary<string, string>> Apis { get; set; } = new Dictionary<string, Dictionary<string, string>>();

        public Dictionary<string, int> PathTraverseOrder { get; set; } = new Dictionary<string, int>();

        public T GetApiSettings<T>(string apiName)
        {
            var json = JsonSerializer.Serialize(Apis[apiName]);
            return JsonSerializer.Deserialize<T>(json);
        }
    }
}