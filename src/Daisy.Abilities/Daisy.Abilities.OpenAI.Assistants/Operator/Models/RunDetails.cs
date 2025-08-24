namespace Daisy.Abilities.Assistant.Operator.Models
{
    public class RunDetails
    {
        public string id { get; set; }
        public string status { get; set; }
        public RequiredAction required_action { get; set; }
    }

    public class RequiredAction
    {
        public SubmitToolOutputs submit_tool_outputs { get; set; }
    }

    public class SubmitToolOutputs
    {
        public ToolCall[] tool_calls { get; set; }
    }

    public class ToolCall
    {
        public string id { get; set; }
        public ToolFunction function { get; set; }
    }

    public class ToolFunction
    {
        public string name { get; set; }
        public string arguments { get; set; }
    }
}
