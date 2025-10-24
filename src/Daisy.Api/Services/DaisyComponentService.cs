// Copyright (c) 2025 Rui Filipe Rodrigues Vaz
// Licensed under the Apache License, Version 2.0

using Daisy.Api.Models;
using Daisy.Resources.Pools;
using Daisy.Resources.Interfaces;

namespace Daisy.Api.Services
{
    /// <summary>
    /// Service for managing and querying Daisy workflow components.
    /// </summary>
    public interface IDaisyComponentService
    {
        Task<IEnumerable<ReceiverDto>> GetReceiversAsync();
        Task<ReceiverDto?> GetReceiverAsync(string name);
        Task<IEnumerable<AbilityDto>> GetAbilitiesAsync();
        Task<AbilityDto?> GetAbilityAsync(string name);
        Task<IEnumerable<TransmitterDto>> GetTransmittersAsync();
        Task<TransmitterDto?> GetTransmitterAsync(string name);
        Task<IEnumerable<WorkflowDto>> GetWorkflowsAsync();
        Task<WorkflowDto?> GetWorkflowAsync(string name);
    }

    /// <summary>
    /// Implementation of Daisy component service.
    /// </summary>
    public class DaisyComponentService : IDaisyComponentService
    {
        public Task<IEnumerable<ReceiverDto>> GetReceiversAsync()
        {
            var receivers = new List<ReceiverDto>();

            try
            {
                foreach (var receiver in ExternalReceivers.Instance.Pool)
                {
                    receivers.Add(new ReceiverDto
                    {
                        Name = receiver.GetType().Name,
                        DisplayName = receiver.GetType().Name,
                        ReceiverType = "External",
                        RunOnCores = receiver.RunOnCores.ToList(),
                        IsEnabled = true
                    });
                }
            }
            catch
            {
                // Pool may not be initialized in test environment
            }

            try
            {
                foreach (var receiver in LoopBackReceivers.Instance.Pool)
                {
                    receivers.Add(new ReceiverDto
                    {
                        Name = receiver.GetType().Name,
                        DisplayName = receiver.GetType().Name,
                        ReceiverType = "LoopBack",
                        RunOnCores = new List<string>(),
                        IsEnabled = true
                    });
                }
            }
            catch
            {
                // Pool may not be initialized in test environment
            }

            try
            {
                foreach (var receiver in EventReceivers.Instance.Pool)
                {
                    receivers.Add(new ReceiverDto
                    {
                        Name = receiver.GetType().Name,
                        DisplayName = receiver.GetType().Name,
                        ReceiverType = "Event",
                        RunOnCores = receiver.RunOnCores.ToList(),
                        IsEnabled = true
                    });
                }
            }
            catch
            {
                // Pool may not be initialized in test environment
            }

            return Task.FromResult<IEnumerable<ReceiverDto>>(receivers);
        }

        public async Task<ReceiverDto?> GetReceiverAsync(string name)
        {
            var receivers = await GetReceiversAsync();
            return receivers.FirstOrDefault(r => r.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        public Task<IEnumerable<AbilityDto>> GetAbilitiesAsync()
        {
            var abilities = new List<AbilityDto>();

            try
            {
                foreach (var path in Paths.Instance.Pool)
                {
                    var abilityName = path.GetType().DeclaringType?.Name ?? path.GetType().Name;
                    
                    var existingAbility = abilities.FirstOrDefault(a => a.Name == abilityName);
                    if (existingAbility == null)
                    {
                        existingAbility = new AbilityDto
                        {
                            Name = abilityName,
                            DisplayName = abilityName,
                            IsEnabled = true
                        };
                        abilities.Add(existingAbility);
                    }

                    existingAbility.Paths.Add(new PathDto
                    {
                        PathName = path.PathName,
                        TraverseOrder = path.TraverseOrder
                    });
                }
            }
            catch
            {
                // Pool may not be initialized in test environment
            }

            return Task.FromResult<IEnumerable<AbilityDto>>(abilities);
        }

        public async Task<AbilityDto?> GetAbilityAsync(string name)
        {
            var abilities = await GetAbilitiesAsync();
            return abilities.FirstOrDefault(a => a.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        public Task<IEnumerable<TransmitterDto>> GetTransmittersAsync()
        {
            var transmitters = new List<TransmitterDto>();

            try
            {
                foreach (var transmitter in ExternalTransmitters.Instance.Pool)
                {
                    transmitters.Add(new TransmitterDto
                    {
                        Name = transmitter.GetType().Name,
                        DisplayName = transmitter.GetType().Name,
                        TransmitterType = "External",
                        IsEnabled = true
                    });
                }
            }
            catch
            {
                // Pool may not be initialized in test environment
            }

            try
            {
                foreach (var transmitter in LoopBackTransmitters.Instance.Pool)
                {
                    transmitters.Add(new TransmitterDto
                    {
                        Name = transmitter.GetType().Name,
                        DisplayName = transmitter.GetType().Name,
                        TransmitterType = "LoopBack",
                        IsEnabled = true
                    });
                }
            }
            catch
            {
                // Pool may not be initialized in test environment
            }

            return Task.FromResult<IEnumerable<TransmitterDto>>(transmitters);
        }

        public async Task<TransmitterDto?> GetTransmitterAsync(string name)
        {
            var transmitters = await GetTransmittersAsync();
            return transmitters.FirstOrDefault(t => t.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        public Task<IEnumerable<WorkflowDto>> GetWorkflowsAsync()
        {
            var workflows = new List<WorkflowDto>();

            try
            {
                foreach (var core in Cores.Instance.Pool)
                {
                    workflows.Add(new WorkflowDto
                    {
                        Name = core.GetType().Name,
                        DisplayName = core.GetType().Name,
                        Status = "Running",
                        IsEnabled = true
                    });
                }
            }
            catch
            {
                // Pool may not be initialized in test environment
            }

            return Task.FromResult<IEnumerable<WorkflowDto>>(workflows);
        }

        public async Task<WorkflowDto?> GetWorkflowAsync(string name)
        {
            var workflows = await GetWorkflowsAsync();
            return workflows.FirstOrDefault(w => w.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }
    }
}
