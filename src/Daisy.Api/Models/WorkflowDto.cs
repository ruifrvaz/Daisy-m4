// Copyright (c) 2025 Rui Filipe Rodrigues Vaz
// Licensed under the Apache License, Version 2.0

using System.ComponentModel.DataAnnotations;

namespace Daisy.Api.Models
{
    /// <summary>
    /// Data Transfer Object for Workflows (Cores) - execution containers that manage workflow lifecycles.
    /// </summary>
    public class WorkflowDto
    {
        /// <summary>
        /// Gets or sets the unique identifier for this workflow.
        /// </summary>
        [Key]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the display name of the workflow.
        /// </summary>
        public string DisplayName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the description of what this workflow does.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets the current status of the workflow (Running, Stopped, Paused).
        /// </summary>
        public string Status { get; set; } = "Stopped";

        /// <summary>
        /// Gets or sets a value indicating whether this workflow is currently enabled.
        /// </summary>
        public bool IsEnabled { get; set; } = true;

        /// <summary>
        /// Gets or sets the timestamp when this workflow was started.
        /// </summary>
        public DateTime? StartedAt { get; set; }

        /// <summary>
        /// Gets or sets the list of receivers associated with this workflow.
        /// </summary>
        public List<string> AssociatedReceivers { get; set; } = new List<string>();
    }
}
