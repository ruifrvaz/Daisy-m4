// Copyright (c) 2025 Rui Filipe Rodrigues Vaz
// Licensed under the Apache License, Version 2.0

using Daisy.Factories;
using Daisy.Resources.Services;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Daisy
{
    class Program
    {
        static void Main(string[] args)
        {
            var settings = StartupFactory.AppSettingsConfiguration.LoadApplicationSettings();

            // Force load plugin assemblies by referencing types from each - ensures assemblies are loaded into AppDomain
            var operatorType = typeof(Daisy.Abilities.Operator.ListWorkflowsPath);
            var terminateType = typeof(Daisy.Abilities.Terminate.TerminatePath);
            var validatorType = typeof(Daisy.Abilities.OutputValidator.Paths.OutputValidatorPath);
            var weatherType = typeof(Daisy.Abilities.Weather.Paths.GetWeatherByCityPath);
            var consoleReceiverType = typeof(Daisy.Receivers.Console.ConsoleReceiver);
            var weatherEventReceiverType = typeof(Daisy.Receivers.WeatherEvent.WeatherEventReceiver);
            var consoleTransmitterType = typeof(Daisy.Transmitters.Console.ConsoleTransmitter);
            var workflowTriggerTransmitterType = typeof(Daisy.Transmitters.WorkflowTrigger.WorkflowTriggerLoopbackTransmitter);
            var starterWorkflowType = typeof(Daisy.Workflows.Starter.StarterCore);
            var weatherWorkflowType = typeof(Daisy.Workflows.Weather.WeatherCore);

            var serviceProvider = StartupFactory.LoadServices(settings);

            AbilityFactory.LoadAbilities(settings, serviceProvider);

            TransmitterFactory.LoadExternalTransmitters(settings, serviceProvider);
            TransmitterFactory.LoadLoopBackTransmitters(settings, serviceProvider);

            ReceiverFactory.LoadExternalReceivers(settings, serviceProvider);
            ReceiverFactory.LoadLoopBackReceivers(settings, serviceProvider);
            ReceiverFactory.LoadEventReceivers(settings, serviceProvider);

            WorkflowFactory.LoadCores(settings, serviceProvider);

            var tokenSource = new CancellationTokenSource();
            var token = tokenSource.Token;

            var tasks = Resources.Pools.Cores.Instance.Pool.Select(core => core.Start(token)).ToArray();
            Task.WaitAll(tasks);

            ServiceContainer.Instance.CleanupServiceProvider();
        }
    }
}