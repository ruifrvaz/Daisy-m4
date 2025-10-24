// Copyright (c) 2025 Rui Filipe Rodrigues Vaz
// Licensed under the Apache License, Version 2.0

using Daisy.Api.Models;
using Daisy.Resources.Signals;
using System.Collections.Concurrent;

namespace Daisy.Api.Services
{
    /// <summary>
    /// Service for managing impulse lifecycle and storage.
    /// In-memory storage for API-created impulses.
    /// </summary>
    public interface IImpulseService
    {
        Task<ImpulseDto> CreateImpulseAsync(string input, bool isLoopback = false);
        Task<ImpulseDto?> GetImpulseAsync(Guid id);
        Task<IEnumerable<ImpulseDto>> GetAllImpulsesAsync();
        Task<ImpulseDto?> UpdateImpulseAsync(Guid id, ImpulseDto impulseDto);
        Task<bool> DeleteImpulseAsync(Guid id);
        Impulse? GetCoreImpulse(Guid id);
        void UpdateFromCoreImpulse(Guid id, Impulse impulse);
    }

    /// <summary>
    /// Implementation of impulse management service.
    /// </summary>
    public class ImpulseService : IImpulseService
    {
        private readonly ConcurrentDictionary<Guid, ImpulseDto> _impulses = new();
        private readonly ConcurrentDictionary<Guid, Impulse> _coreImpulses = new();

        public Task<ImpulseDto> CreateImpulseAsync(string input, bool isLoopback = false)
        {
            var impulseDto = new ImpulseDto
            {
                Id = Guid.NewGuid(),
                Input = input,
                IsLoopback = isLoopback,
                CreatedAt = DateTime.UtcNow,
                Status = "Created"
            };

            _impulses[impulseDto.Id] = impulseDto;

            var coreImpulse = new Impulse
            {
                Input = input,
                IsLoopback = isLoopback
            };

            _coreImpulses[impulseDto.Id] = coreImpulse;

            return Task.FromResult(impulseDto);
        }

        public Task<ImpulseDto?> GetImpulseAsync(Guid id)
        {
            _impulses.TryGetValue(id, out var impulse);
            return Task.FromResult(impulse);
        }

        public Task<IEnumerable<ImpulseDto>> GetAllImpulsesAsync()
        {
            return Task.FromResult<IEnumerable<ImpulseDto>>(_impulses.Values.ToList());
        }

        public Task<ImpulseDto?> UpdateImpulseAsync(Guid id, ImpulseDto impulseDto)
        {
            if (_impulses.TryGetValue(id, out var existing))
            {
                existing.Output = impulseDto.Output;
                existing.Error = impulseDto.Error;
                existing.Status = impulseDto.Status;
                existing.UpdatedAt = DateTime.UtcNow;
                existing.TraversedPathNames = impulseDto.TraversedPathNames;

                return Task.FromResult<ImpulseDto?>(existing);
            }

            return Task.FromResult<ImpulseDto?>(null);
        }

        public Task<bool> DeleteImpulseAsync(Guid id)
        {
            var removed = _impulses.TryRemove(id, out _);
            if (removed)
            {
                _coreImpulses.TryRemove(id, out _);
            }
            return Task.FromResult(removed);
        }

        public Impulse? GetCoreImpulse(Guid id)
        {
            _coreImpulses.TryGetValue(id, out var impulse);
            return impulse;
        }

        public void UpdateFromCoreImpulse(Guid id, Impulse impulse)
        {
            _coreImpulses[id] = impulse;

            if (_impulses.TryGetValue(id, out var impulseDto))
            {
                impulseDto.Input = impulse.Input;
                impulseDto.Output = impulse.Output;
                impulseDto.Error = impulse.Error;
                impulseDto.IsLoopback = impulse.IsLoopback;
                impulseDto.UpdatedAt = DateTime.UtcNow;
                impulseDto.Status = string.IsNullOrEmpty(impulse.Error) ? "Processed" : "Error";
                impulseDto.TraversedPathNames = impulse.TraversedPaths.Select(p => p.PathName).ToList();
            }
        }
    }
}
