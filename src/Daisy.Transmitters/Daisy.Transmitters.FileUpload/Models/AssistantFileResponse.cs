using System;

namespace Daisy.Transmitters.VectorStore.Models
{
    internal class AssistantFileResponse
    {
        public string id { get; set; }
        public string @object { get; set; }
        public long created_at { get; set; }
        public string assistant_id { get; set; }
    }
}
