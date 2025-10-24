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
    /// OData controller for managing Abilities - processing units that transform impulses.
    /// Provides read-only access to abilities and their paths, plus execution capabilities.
    /// </summary>
    [ApiController]
    [Route("odata/[controller]")]
    [Authorize]
    public class AbilitiesController : ODataController
    {
        private readonly IDaisyComponentService _componentService;
        private readonly IImpulseService _impulseService;
        private readonly ILogger<AbilitiesController> _logger;

        public AbilitiesController(
            IDaisyComponentService componentService,
            IImpulseService impulseService,
            ILogger<AbilitiesController> logger)
        {
            _componentService = componentService;
            _impulseService = impulseService;
            _logger = logger;
        }

        /// <summary>
        /// Gets all abilities with OData query support.
        /// </summary>
        /// <returns>A queryable collection of abilities.</returns>
        [HttpGet]
        [EnableQuery]
        public async Task<ActionResult<IEnumerable<AbilityDto>>> Get()
        {
            _logger.LogInformation("Getting all abilities");
            var abilities = await _componentService.GetAbilitiesAsync();
            return Ok(abilities);
        }

        /// <summary>
        /// Gets a specific ability by name.
        /// </summary>
        /// <param name="key">The ability name.</param>
        /// <returns>The requested ability or 404 if not found.</returns>
        [HttpGet("{key}")]
        [EnableQuery]
        public async Task<ActionResult<AbilityDto>> Get([FromRoute] string key)
        {
            _logger.LogInformation("Getting ability: {AbilityName}", LogSanitizer.Sanitize(key));
            var ability = await _componentService.GetAbilityAsync(key);
            
            if (ability == null)
            {
                return NotFound();
            }

            return Ok(ability);
        }

        /// <summary>
        /// Executes an ability with a specified impulse.
        /// This is a custom action beyond standard OData operations.
        /// </summary>
        /// <param name="key">The ability name to execute.</param>
        /// <param name="request">The execution request with impulse ID.</param>
        /// <returns>The processed impulse.</returns>
        [HttpPost("{key}/Execute")]
        public async Task<ActionResult<ImpulseDto>> Execute([FromRoute] string key, [FromBody] ExecuteAbilityRequest request)
        {
            _logger.LogInformation("Executing ability: {AbilityName} with impulse: {ImpulseId}", LogSanitizer.Sanitize(key), LogSanitizer.Sanitize(request.ImpulseId));
            
            var ability = await _componentService.GetAbilityAsync(key);
            if (ability == null)
            {
                return NotFound($"Ability '{key}' not found");
            }

            var impulse = await _impulseService.GetImpulseAsync(request.ImpulseId);
            if (impulse == null)
            {
                return NotFound($"Impulse '{request.ImpulseId}' not found");
            }

            impulse.Status = "Processing";
            impulse.UpdatedAt = DateTime.UtcNow;

            _logger.LogInformation("Executing ability {AbilityName} on impulse {ImpulseId}", LogSanitizer.Sanitize(key), LogSanitizer.Sanitize(request.ImpulseId));
            
            return Ok(impulse);
        }
    }
}
