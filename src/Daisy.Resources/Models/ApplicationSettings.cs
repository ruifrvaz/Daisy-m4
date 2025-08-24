using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Daisy.Resources.Models
{
    public class Root
    {
        public ApplicationSettings ApplicationSettings { get; set; }
    }

    public class ApplicationSettings
    {
        public string SolutionName { get; set; }

        public Dictionary<string, ReceiverConfiguration> Receivers { get; set; } = new Dictionary<string, ReceiverConfiguration>();

        public List<string> Transmitters { get; set; } = new List<string>();

        public List<string> Abilities { get; set; } = new List<string>();

        public List<string> Workflows { get; set; } = new List<string>();

        public Dictionary<string, Dictionary<string, string>> Apis { get; set; } = new Dictionary<string, Dictionary<string, string>>();

        public Dictionary<string, int> PathTraverseOrder { get; set; } = new Dictionary<string, int>();

        public T GetApiSettings<T>(string apiName)
        {
            var json = JsonConvert.SerializeObject(Apis[apiName]);
            return JsonConvert.DeserializeObject<T>(json);
        }
    }

    public class ReceiverConfiguration
    {
        public List<string> RunOnCores { get; set; } = new List<string>();
    }
}