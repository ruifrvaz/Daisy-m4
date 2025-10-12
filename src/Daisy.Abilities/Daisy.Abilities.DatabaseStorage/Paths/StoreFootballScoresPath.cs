using Daisy.Abilities.DatabaseStorage.Services;
using Daisy.Resources.Abstracts;
using Daisy.Resources.Extensions;
using Daisy.Resources.Interfaces;
using Daisy.Resources.Models;
using Daisy.Resources.Services;
using Daisy.Resources.Signals;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Daisy.Abilities.DatabaseStorage.Paths
{
    /// <summary>
    /// Path for storing football scores in a database.
    /// Traverses when output chain contains "footballScores: {scores}".
    /// </summary>
    public class StoreFootballScoresPath : APath
    {
        private readonly IDatabaseService? _databaseService;

        public StoreFootballScoresPath(IServiceProvider serviceProvider,
            IEnumerable<ITraverseRule> traverseRules,
            IEnumerable<ITraverseRule> hasBeenTraversedRules,
            string pathName,
            int traverseOrder,
            ApplicationSettings settings)
            : base(serviceProvider, traverseRules, hasBeenTraversedRules, pathName, traverseOrder, settings)
        {
            _databaseService = ServiceContainer.Instance.GetService<IDatabaseService>() as IDatabaseService;
        }

        public override async Task Traverse(Impulse impulse)
        {
            var clubName = impulse.GetChainByKey("football");
            var scores = impulse.GetChainByKey("footballScores", ImpulseExtensions.ImpulseField.Output);

            if (string.IsNullOrWhiteSpace(scores) || string.IsNullOrWhiteSpace(clubName))
            {
                impulse.Error = "No football scores available to store in database.";
            }
            else
            {
                var stored = await _databaseService!.StoreFootballScoresAsync(clubName, scores);
                
                if (stored)
                {
                    impulse.AddChain($"databaseStored: Scores stored for {clubName}", ImpulseExtensions.ImpulseField.Output);
                }
                else
                {
                    impulse.Error = $"Failed to store scores for {clubName} in database.";
                }
            }

            await Emit(impulse);
        }
    }
}
