namespace Daisy.Transmitters.AzureDevOps.Models
{
    internal class AzureDevOpsSettings
    {
        public string Organization { get; set; } = string.Empty;
        public string Project { get; set; } = string.Empty;
        public string PersonalAccessToken { get; set; } = string.Empty;
    }
}
