using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daisy.Abilities.Assistant.StoryParser.Models
{
    internal class ThreadResponse
    {
        public string id { get; set; }

        public string @object { get; set; }

        public long created_at { get; set; }

        public object metadata { get; set; }
    }
}
