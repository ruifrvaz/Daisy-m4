using Daisy.Abilities.Football.Services;
using Daisy.Resources.Abstracts;
using Daisy.Resources.Extensions;
using Daisy.Resources.Interfaces;
using Daisy.Resources.Models;
using Daisy.Resources.Services;
using Daisy.Resources.Signals;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Daisy.Abilities.Football.Paths
{
    /// <summary>
    /// Path for fetching football scores for a club.
    /// Traverses when input chain contains "football: {clubName}".
    /// </summary>
    public class GetFootballScoresPath : APath
    {
        private readonly IFootballService? _footballService;

        public GetFootballScoresPath(IServiceProvider serviceProvider,
            IEnumerable<ITraverseRule> traverseRules,
            IEnumerable<ITraverseRule> hasBeenTraversedRules,
            string pathName,
            int traverseOrder,
            ApplicationSettings settings)
            : base(serviceProvider, traverseRules, hasBeenTraversedRules, pathName, traverseOrder, settings)
        {
            _footballService = ServiceContainer.Instance.GetService<IFootballService>() as IFootballService;
        }

        public override async Task Traverse(Impulse impulse)
        {
            var clubName = impulse.GetChainByKey("football");
            var scores = clubName == null ? null : await _footballService!.GetFootballScoresAsync(clubName);

            if (string.IsNullOrWhiteSpace(scores))
            {
                impulse.Error = $"Unable to retrieve scores for {clubName}.";
            }
            else
            {
                impulse.AddChain($"footballScores: {scores}", ImpulseExtensions.ImpulseField.Output);
            }

            await Emit(impulse);
        }
    }
}
