using System.Collections.Generic;

namespace Daisy.Transmitters.AzureDevOps.Models
{
    public class UserStory
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<string> AcceptanceCriteria { get; set; } = new();
        public string Repository { get; set; } = string.Empty;
        public string Branch { get; set; } = string.Empty;
        public string Application { get; set; } = string.Empty;
        public string Pipeline { get; set; } = string.Empty;
    }
}
