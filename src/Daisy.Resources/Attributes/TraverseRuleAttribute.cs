using System;

namespace Daisy.Resources.Attributes
{

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class TraverseRuleAttribute : Attribute
    {
        public Type PathType { get; set; }
    }
}