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
    /// OData controller for managing Workflows (Cores) - execution containers that manage workflow lifecycles.
    /// Provides read-only access to workflows and control capabilities.
    /// </summary>
    [ApiController]
    [Route("odata/[controller]")]
    [Authorize]
    public class WorkflowsController : ODataController
    {
        private readonly IDaisyComponentService _componentService;
        private readonly ILogger<WorkflowsController> _logger;

        public WorkflowsController(
            IDaisyComponentService componentService,
            ILogger<WorkflowsController> logger)
        {
            _componentService = componentService;
            _logger = logger;
        }

        /// <summary>
        /// Gets all workflows with OData query support.
        /// </summary>
        /// <returns>A queryable collection of workflows.</returns>
        [HttpGet]
        [EnableQuery]
        public async Task<ActionResult<IEnumerable<WorkflowDto>>> Get()
        {
            _logger.LogInformation("Getting all workflows");
            var workflows = await _componentService.GetWorkflowsAsync();
            return Ok(workflows);
        }

        /// <summary>
        /// Gets a specific workflow by name.
        /// </summary>
        /// <param name="key">The workflow name.</param>
        /// <returns>The requested workflow or 404 if not found.</returns>
        [HttpGet("{key}")]
        [EnableQuery]
        public async Task<ActionResult<WorkflowDto>> Get([FromRoute] string key)
        {
            _logger.LogInformation("Getting workflow: {WorkflowName}", key);
            var workflow = await _componentService.GetWorkflowAsync(key);
            
            if (workflow == null)
            {
                return NotFound();
            }

            return Ok(workflow);
        }

        /// <summary>
        /// Gets the status of a specific workflow.
        /// This is a custom function beyond standard OData operations.
        /// </summary>
        /// <param name="key">The workflow name.</param>
        /// <returns>The workflow status.</returns>
        [HttpGet("{key}/Status")]
        public async Task<ActionResult<WorkflowControlResponse>> GetStatus([FromRoute] string key)
        {
            _logger.LogInformation("Getting workflow status: {WorkflowName}", key);
            
            var workflow = await _componentService.GetWorkflowAsync(key);
            if (workflow == null)
            {
                return NotFound($"Workflow '{key}' not found");
            }

            return Ok(new WorkflowControlResponse
            {
                WorkflowName = workflow.Name,
                Status = workflow.Status,
                Success = true,
                Message = $"Workflow '{key}' is {workflow.Status}"
            });
        }
    }
}
