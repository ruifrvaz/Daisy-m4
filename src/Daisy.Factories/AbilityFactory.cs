using Daisy.Resources.Attributes;
using Daisy.Resources.Interfaces;
using Daisy.Resources.Models;
using Daisy.Resources.Pools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Daisy.Factories
{
    public sealed class AbilityFactory
    {
        public static void LoadAbility(ApplicationSettings settings, IServiceProvider serviceProvider, string abilityName)
        {
            var abilityInterface = typeof(IPath);
            var traverseRuleInterface = typeof(ITraverseRule);

            // load the ability assembly
            var assembly = Assembly.LoadFrom($"Daisy.Abilities.Assistant.{abilityName}.dll");
            var abilityPaths = assembly.GetTypes()
                                .Where(type => abilityInterface.IsAssignableFrom(type) && type.IsClass);

            foreach (var abilityPath in abilityPaths)
            {
                // load the traverse rules for the current ability
                var traverseRuleTypes = assembly
                                        .GetTypes()
                                        .Where(type => traverseRuleInterface.IsAssignableFrom(type) && type.IsClass);

                var traverseRules = LoadCanTraverseRules(traverseRuleTypes, abilityPath, settings);

                var hasBeenTraversedRules = LoadTraversedRules(traverseRuleTypes, abilityPath, settings);

                var traverseOrder = settings.PathTraverseOrder[abilityPath.Name];

                dynamic pathObject = Activator.CreateInstance(abilityPath, serviceProvider, traverseRules, hasBeenTraversedRules, abilityPath.Name, traverseOrder, settings);
                var path = pathObject as IPath;
                Paths.Instance.Pool.Add(path);
            }
        }

        // find all classes that implement IPath
        public static void LoadAbilities(ApplicationSettings settings, IServiceProvider serviceProvider)
        {
            var abilityInterface = typeof(IPath);
            var traverseRuleInterface = typeof(ITraverseRule);

            // Load known ability path types directly
            LoadAbilityPath<Daisy.Abilities.Operator.ListWorkflowsPath>(settings, serviceProvider, abilityInterface, traverseRuleInterface);
            LoadAbilityPath<Daisy.Abilities.Operator.Paths.WorkflowTriggerPath>(settings, serviceProvider, abilityInterface, traverseRuleInterface);
            LoadAbilityPath<Daisy.Abilities.OutputValidator.Paths.OutputValidatorPath>(settings, serviceProvider, abilityInterface, traverseRuleInterface);
            LoadAbilityPath<Daisy.Abilities.Weather.Paths.GetWeatherByCityPath>(settings, serviceProvider, abilityInterface, traverseRuleInterface);
            LoadAbilityPath<Daisy.Abilities.Terminate.TerminatePath>(settings, serviceProvider, abilityInterface, traverseRuleInterface);
        }

        private static void LoadAbilityPath<T>(ApplicationSettings settings, IServiceProvider serviceProvider, Type abilityInterface, Type traverseRuleInterface) where T : class
        {
            var abilityPath = typeof(T);
            
            if (!abilityInterface.IsAssignableFrom(abilityPath) || !abilityPath.IsClass || abilityPath.IsAbstract)
                return;

            if (!settings.PathTraverseOrder.ContainsKey(abilityPath.Name))
                return;

            // Get traverse rule types from the same assembly
            var assembly = abilityPath.Assembly;
            var types = GetExportedTypesFromAssembly(assembly);
            var traverseRuleTypes = types.Where(t => t is { IsClass: true, IsAbstract: false } && traverseRuleInterface.IsAssignableFrom(t));

            var traverseRules = LoadCanTraverseRules(traverseRuleTypes, abilityPath, settings);
            var hasBeenTraversedRules = LoadTraversedRules(traverseRuleTypes, abilityPath, settings);
            var traverseOrder = settings.PathTraverseOrder[abilityPath.Name];

            // ctor: (IServiceProvider, traverseRules, hasBeenTraversedRules, name, order, settings)
            var instance = (IPath)Activator.CreateInstance(
                abilityPath, serviceProvider, traverseRules, hasBeenTraversedRules,
                abilityPath.Name, traverseOrder, settings)!;

            Paths.Instance.Pool.Add(instance);
        }

        private static Type[] GetExportedTypesFromAssembly(Assembly assembly)
        {
            try
            {
                return assembly.GetExportedTypes();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting types from assembly {assembly.GetName().Name}: {ex.Message}");
                return new Type[0];
            }
        }

        private static IEnumerable<ITraverseRule> LoadCanTraverseRules(IEnumerable<Type> traverseRuleTypes, Type pathType, ApplicationSettings settings)
        {
            var pathTraverseRuleTypes = from traverseRuleType in traverseRuleTypes
                                        from attribute in traverseRuleType.GetCustomAttributes(typeof(TraverseRuleAttribute), false)
                                        where ((TraverseRuleAttribute)attribute).PathType == pathType
                                        select traverseRuleType;

            var traverseRules = new List<ITraverseRule>();
            foreach (var traverseRuleType in pathTraverseRuleTypes)
            {
                dynamic traverseRuleObject = Activator.CreateInstance(traverseRuleType, settings);
                var traverseRule = traverseRuleObject as ITraverseRule;
                if (traverseRule != null)
                {
                    traverseRules.Add(traverseRule);
                }
            }

            return traverseRules;
        }

        private static IEnumerable<ITraverseRule> LoadTraversedRules(IEnumerable<Type> hasBeenTraversedRuleTypes, Type pathType, ApplicationSettings settings)
        {
            var pathHasBeenTraversedRuleTypes = from hasBeenTraversedRuleType in hasBeenTraversedRuleTypes
                                                from attribute in hasBeenTraversedRuleType.GetCustomAttributes(typeof(TraversedRuleAttribute), false)
                                                where ((TraversedRuleAttribute)attribute).PathType == pathType
                                                select hasBeenTraversedRuleType;

            var hasBeenTraversedRules = new List<ITraverseRule>();
            foreach (var hasBeenTraversedRuleType in pathHasBeenTraversedRuleTypes)
            {
                dynamic traverseRuleObject = Activator.CreateInstance(hasBeenTraversedRuleType, settings);
                var traverseRule = traverseRuleObject as ITraverseRule;
                if (traverseRule != null)
                {
                    hasBeenTraversedRules.Add(traverseRule);
                }
            }
            return hasBeenTraversedRules;
        }

    }
}