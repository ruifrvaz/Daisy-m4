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
    /// OData controller for managing Receivers - entry points that ingest external input.
    /// Provides read-only access to receivers and trigger capabilities.
    /// </summary>
    [ApiController]
    [Route("odata/[controller]")]
    [Authorize]
    public class ReceiversController : ODataController
    {
        private readonly IDaisyComponentService _componentService;
        private readonly IImpulseService _impulseService;
        private readonly ILogger<ReceiversController> _logger;

        public ReceiversController(
            IDaisyComponentService componentService,
            IImpulseService impulseService,
            ILogger<ReceiversController> logger)
        {
            _componentService = componentService;
            _impulseService = impulseService;
            _logger = logger;
        }

        /// <summary>
        /// Gets all receivers with OData query support.
        /// </summary>
        /// <returns>A queryable collection of receivers.</returns>
        [HttpGet]
        [EnableQuery]
        public async Task<ActionResult<IEnumerable<ReceiverDto>>> Get()
        {
            _logger.LogInformation("Getting all receivers");
            var receivers = await _componentService.GetReceiversAsync();
            return Ok(receivers);
        }

        /// <summary>
        /// Gets a specific receiver by name.
        /// </summary>
        /// <param name="key">The receiver name.</param>
        /// <returns>The requested receiver or 404 if not found.</returns>
        [HttpGet("{key}")]
        [EnableQuery]
        public async Task<ActionResult<ReceiverDto>> Get([FromRoute] string key)
        {
            _logger.LogInformation("Getting receiver: {ReceiverName}", key);
            var receiver = await _componentService.GetReceiverAsync(key);
            
            if (receiver == null)
            {
                return NotFound();
            }

            return Ok(receiver);
        }

        /// <summary>
        /// Triggers a receiver with input data to create and process an impulse.
        /// This is a custom action beyond standard OData operations.
        /// </summary>
        /// <param name="key">The receiver name to trigger.</param>
        /// <param name="request">The trigger request with input data.</param>
        /// <returns>The created impulse.</returns>
        [HttpPost("{key}/Trigger")]
        public async Task<ActionResult<ImpulseDto>> Trigger([FromRoute] string key, [FromBody] TriggerReceiverRequest request)
        {
            _logger.LogInformation("Triggering receiver: {ReceiverName}", key);
            
            var receiver = await _componentService.GetReceiverAsync(key);
            if (receiver == null)
            {
                return NotFound($"Receiver '{key}' not found");
            }

            var impulse = await _impulseService.CreateImpulseAsync(request.Input, false);
            impulse.Status = "Processing";

            _logger.LogInformation("Created impulse {ImpulseId} from receiver {ReceiverName}", impulse.Id, key);
            
            return Created($"/odata/Impulses/{impulse.Id}", impulse);
        }
    }
}
