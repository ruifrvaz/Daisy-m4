using Daisy.Abilities.LocalModel.Services;
using Daisy.Resources.Abstracts;
using Daisy.Resources.Extensions;
using Daisy.Resources.Interfaces;
using Daisy.Resources.Models;
using Daisy.Resources.Services;
using Daisy.Resources.Signals;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Daisy.Abilities.LocalModel.Paths
{
    public class GetLocalModelResponsePath : APath
    {
        private readonly ILocalModelService _localModelService;

        public GetLocalModelResponsePath(IServiceProvider serviceProvider,
            IEnumerable<ITraverseRule> traverseRules,
            IEnumerable<ITraverseRule> hasBeenTraversedRules,
            string pathName,
            int traverseOrder,
            ApplicationSettings settings)
            : base(serviceProvider, traverseRules, hasBeenTraversedRules, pathName, traverseOrder, settings)
        {
            _localModelService = ServiceContainer.Instance.GetService<ILocalModelService>() as ILocalModelService;
        }

        public override async Task Traverse(Impulse impulse)
        {
            var prompt = impulse.GetChainByKey("localmodel");
            var response = prompt == null ? null : await _localModelService.GetResponseAsync(prompt);

            if (string.IsNullOrWhiteSpace(response))
            {
                impulse.Error = $"Unable to get response from local model for prompt: {prompt}.";
            }
            else
            {
                impulse.AddChain($"LocalModel: {response}", ImpulseExtensions.ImpulseField.Output);
            }

            await Emit(impulse);
        }
    }
}
