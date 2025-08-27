using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daisy.Abilities.Assistant.Operator.Models
{
    public class ToolOutput
    {
        public string tool_call_id { get; set; }

        public string output { get; set; }
    }
}
