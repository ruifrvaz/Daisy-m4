using Daisy.Resources.Pools;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Daisy.Resources.Helpers
{
    public class WorkflowFinder
    {
        public static IReadOnlyList<string> GetAvailableWorkflowIdentifiers()
        {
            return Cores.Instance.Pool
                .Select(core => core.GetType().Namespace)
                .Select(ns => ns.Split('.')).Last()
                .Where(name => !name.Equals("Starter", StringComparison.InvariantCultureIgnoreCase))
                .Distinct(StringComparer.InvariantCultureIgnoreCase)
                .ToList();
        }
    }
}