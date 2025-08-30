using Daisy.Abilities.HealthCheck.Services;
using Daisy.Resources.Abstracts;
using Daisy.Resources.Extensions;
using Daisy.Resources.Interfaces;
using Daisy.Resources.Models;
using Daisy.Resources.Services;
using Daisy.Resources.Signals;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Daisy.Abilities.HealthCheck.Models;

namespace Daisy.Abilities.HealthCheck.Paths
{
    public class HealthCheckPath : APath
    {
        private readonly IHealthCheckService _service;

        public HealthCheckPath(IServiceProvider serviceProvider,
            IEnumerable<ITraverseRule> traverseRules,
            IEnumerable<ITraverseRule> hasBeenTraversedRules,
            string pathName,
            int traverseOrder,
            ApplicationSettings settings)
            : base(serviceProvider, traverseRules, hasBeenTraversedRules, pathName, traverseOrder, settings)
        {
            _service = ServiceContainer.Instance.GetService<IHealthCheckService>() as IHealthCheckService;
        }

        public override async Task Traverse(Impulse impulse)
        {
            var storyJson = impulse.Output.GetChainByKey("StoryParsed");
            var story = storyJson.GetData<UserStory>();

            var url = string.Empty;
            if (story != null && !string.IsNullOrWhiteSpace(story.Application))
            {
                url = $"https://{story.Application}rfrv.azurewebsites.net/api/{story.Application}";
            }
            else
            {
                impulse.Error = "Error in health check.";
                await Emit(impulse);
            }

            var healthy = await _service.CheckHealthAsync(url);

            var status = healthy ? "healthy" : "unhealthy";
            impulse.Output = impulse.Output.AddPrefix($"HealthCheck: {status} > ");

            Console.WriteLine($"15. Application status: {status}.");
            await Emit(impulse);
        }
    }
}