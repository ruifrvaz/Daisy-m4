using System;

namespace Daisy.Resources.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class RunOnCoresAttribute : Attribute
    {
        public string[] Cores { get; }

        public RunOnCoresAttribute(params string[] cores)
        {
            Cores = cores;
        }
    }
}
