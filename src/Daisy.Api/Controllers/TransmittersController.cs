// Copyright (c) 2025 Rui Filipe Rodrigues Vaz
// Licensed under the Apache License, Version 2.0

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.AspNetCore.Authorization;
using Daisy.Api.Models;
using Daisy.Api.Services;
using Daisy.Api.Helpers;

namespace Daisy.Api.Controllers
{
    /// <summary>
    /// OData controller for managing Transmitters - output processors that send results to external systems.
    /// Provides read-only access to transmitters and transmission capabilities.
    /// </summary>
    [ApiController]
    [Route("odata/[controller]")]
    [Authorize]
    public class TransmittersController : ODataController
    {
        private readonly IDaisyComponentService _componentService;
        private readonly IImpulseService _impulseService;
        private readonly ILogger<TransmittersController> _logger;

        public TransmittersController(
            IDaisyComponentService componentService,
            IImpulseService impulseService,
            ILogger<TransmittersController> logger)
        {
            _componentService = componentService;
            _impulseService = impulseService;
            _logger = logger;
        }

        /// <summary>
        /// Gets all transmitters with OData query support.
        /// </summary>
        /// <returns>A queryable collection of transmitters.</returns>
        [HttpGet]
        [EnableQuery]
        public async Task<ActionResult<IEnumerable<TransmitterDto>>> Get()
        {
            _logger.LogInformation("Getting all transmitters");
            var transmitters = await _componentService.GetTransmittersAsync();
            return Ok(transmitters);
        }

        /// <summary>
        /// Gets a specific transmitter by name.
        /// </summary>
        /// <param name="key">The transmitter name.</param>
        /// <returns>The requested transmitter or 404 if not found.</returns>
        [HttpGet("{key}")]
        [EnableQuery]
        public async Task<ActionResult<TransmitterDto>> Get([FromRoute] string key)
        {
            _logger.LogInformation("Getting transmitter: {TransmitterName}", LogSanitizer.Sanitize(key));
            var transmitter = await _componentService.GetTransmitterAsync(key);
            
            if (transmitter == null)
            {
                return NotFound();
            }

            return Ok(transmitter);
        }

        /// <summary>
        /// Transmits an impulse through a transmitter.
        /// This is a custom action beyond standard OData operations.
        /// </summary>
        /// <param name="key">The transmitter name to use.</param>
        /// <param name="request">The transmit request with impulse ID.</param>
        /// <returns>The transmitted impulse.</returns>
        [HttpPost("{key}/Transmit")]
        public async Task<ActionResult<ImpulseDto>> Transmit([FromRoute] string key, [FromBody] TransmitRequest request)
        {
            _logger.LogInformation("Transmitting through transmitter: {TransmitterName} with impulse: {ImpulseId}", LogSanitizer.Sanitize(key), LogSanitizer.Sanitize(request.ImpulseId));
            
            var transmitter = await _componentService.GetTransmitterAsync(key);
            if (transmitter == null)
            {
                return NotFound($"Transmitter '{key}' not found");
            }

            var impulse = await _impulseService.GetImpulseAsync(request.ImpulseId);
            if (impulse == null)
            {
                return NotFound($"Impulse '{request.ImpulseId}' not found");
            }

            impulse.Status = "Transmitted";
            impulse.UpdatedAt = DateTime.UtcNow;

            _logger.LogInformation("Transmitted impulse {ImpulseId} through transmitter {TransmitterName}", LogSanitizer.Sanitize(request.ImpulseId), LogSanitizer.Sanitize(key));
            
            return Ok(impulse);
        }
    }
}
