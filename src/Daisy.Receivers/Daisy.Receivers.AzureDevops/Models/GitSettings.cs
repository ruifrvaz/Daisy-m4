namespace Daisy.Receivers.AzureDevopsReceiver.Models
{
    public class GitSettings
    {
        public string Organization { get; set; }
        public string Project { get; set; }
        public string PersonalAccessToken { get; set; }
    }
}
