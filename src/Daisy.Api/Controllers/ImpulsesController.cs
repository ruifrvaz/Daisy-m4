// Copyright (c) 2025 Rui Filipe Rodrigues Vaz
// Licensed under the Apache License, Version 2.0

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.AspNetCore.Authorization;
using Daisy.Api.Models;
using Daisy.Api.Services;

namespace Daisy.Api.Controllers
{
    /// <summary>
    /// OData controller for managing Impulses - the central data carriers in Daisy workflows.
    /// Supports standard CRUD operations with OData query capabilities.
    /// </summary>
    [ApiController]
    [Route("odata/[controller]")]
    [Authorize]
    public class ImpulsesController : ODataController
    {
        private readonly IImpulseService _impulseService;
        private readonly ILogger<ImpulsesController> _logger;

        public ImpulsesController(IImpulseService impulseService, ILogger<ImpulsesController> logger)
        {
            _impulseService = impulseService;
            _logger = logger;
        }

        /// <summary>
        /// Gets all impulses with OData query support.
        /// </summary>
        /// <returns>A queryable collection of impulses.</returns>
        [HttpGet]
        [EnableQuery]
        public async Task<ActionResult<IEnumerable<ImpulseDto>>> Get()
        {
            _logger.LogInformation("Getting all impulses");
            var impulses = await _impulseService.GetAllImpulsesAsync();
            return Ok(impulses);
        }

        /// <summary>
        /// Gets a specific impulse by ID.
        /// </summary>
        /// <param name="key">The impulse ID.</param>
        /// <returns>The requested impulse or 404 if not found.</returns>
        [HttpGet("{key}")]
        [EnableQuery]
        public async Task<ActionResult<ImpulseDto>> Get([FromRoute] Guid key)
        {
            _logger.LogInformation("Getting impulse with ID: {ImpulseId}", key);
            var impulse = await _impulseService.GetImpulseAsync(key);
            
            if (impulse == null)
            {
                return NotFound();
            }

            return Ok(impulse);
        }

        /// <summary>
        /// Creates a new impulse.
        /// </summary>
        /// <param name="impulseDto">The impulse data to create.</param>
        /// <returns>The created impulse.</returns>
        [HttpPost]
        public async Task<ActionResult<ImpulseDto>> Post([FromBody] ImpulseDto impulseDto)
        {
            _logger.LogInformation("Creating new impulse");
            var created = await _impulseService.CreateImpulseAsync(impulseDto.Input, impulseDto.IsLoopback);
            return Created($"/odata/Impulses/{created.Id}", created);
        }

        /// <summary>
        /// Updates an existing impulse.
        /// </summary>
        /// <param name="key">The impulse ID.</param>
        /// <param name="impulseDto">The updated impulse data.</param>
        /// <returns>The updated impulse or 404 if not found.</returns>
        [HttpPut("{key}")]
        public async Task<ActionResult<ImpulseDto>> Put([FromRoute] Guid key, [FromBody] ImpulseDto impulseDto)
        {
            _logger.LogInformation("Updating impulse with ID: {ImpulseId}", key);
            var updated = await _impulseService.UpdateImpulseAsync(key, impulseDto);
            
            if (updated == null)
            {
                return NotFound();
            }

            return Ok(updated);
        }

        /// <summary>
        /// Deletes an impulse.
        /// </summary>
        /// <param name="key">The impulse ID.</param>
        /// <returns>204 No Content if successful, 404 if not found.</returns>
        [HttpDelete("{key}")]
        public async Task<IActionResult> Delete([FromRoute] Guid key)
        {
            _logger.LogInformation("Deleting impulse with ID: {ImpulseId}", key);
            var deleted = await _impulseService.DeleteImpulseAsync(key);
            
            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
