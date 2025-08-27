using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daisy.Abilities.Assistant.StoryParser.Models
{
    internal class RunResponse
    {
        public string id { get; set; }
        public string _object { get; set; }
        public long? created_at { get; set; }
        public string assistant_id { get; set; }
        public string thread_id { get; set; }
        public string status { get; set; }
        public long? started_at { get; set; }
        public long? expires_at { get; set; }
        public long? cancelled_at { get; set; }
        public long? failed_at { get; set; }
        public long? completed_at { get; set; }
        public object last_error { get; set; }
        public string model { get; set; }
        public string instructions { get; set; }
        public Tool[] tools { get; set; }
        public object[] file_ids { get; set; }
        public Metadata metadata { get; set; }
    }

    public class Tool
    {
        public string type { get; set; }
    }
}
