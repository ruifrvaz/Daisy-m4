namespace Daisy.Abilities.Assistant.Operator.Models
{
    public class ThreadConversation
    {

        public string _object { get; set; }
        public Datum[] data { get; set; }
        public string first_id { get; set; }
        public string last_id { get; set; }
        public bool has_more { get; set; }
    }

    public class Datum
    {
        public string id { get; set; }
        public string _object { get; set; }
        public int created_at { get; set; }
        public string thread_id { get; set; }
        public string role { get; set; }
        public Content[] content { get; set; }
        public object[] file_ids { get; set; }
        public string assistant_id { get; set; }
        public string run_id { get; set; }
        public Metadata metadata { get; set; }
    }

    public class Metadata
    {
    }

    public class Content
    {
        public string type { get; set; }
        public Text text { get; set; }
    }

    public class Text
    {
        public string value { get; set; }
        public object[] annotations { get; set; }
    }
}
