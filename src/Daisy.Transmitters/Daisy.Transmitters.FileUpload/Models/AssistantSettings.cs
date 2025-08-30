namespace Daisy.Transmitters.VectorStore.Models
{
    public class AssistantSettings
    {
        public string ApiKey { get; set; }

        public string ApiBaseUrl { get; set; }

        public string AssistantCoderName { get; set; }

        /// <summary>
        /// types: gpt-4, gpt-4-1106-preview
        /// </summary>
        public string GptModel { get; set; }
    }
}