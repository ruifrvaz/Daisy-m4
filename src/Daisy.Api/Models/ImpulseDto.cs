// Copyright (c) 2025 Rui Filipe Rodrigues Vaz
// Licensed under the Apache License, Version 2.0

using System.ComponentModel.DataAnnotations;

namespace Daisy.Api.Models
{
    /// <summary>
    /// Data Transfer Object for Impulse - the central data carrier in Daisy workflows.
    /// Represents the state and data of a workflow execution at a point in time.
    /// </summary>
    public class ImpulseDto
    {
        /// <summary>
        /// Gets or sets the unique identifier for this impulse.
        /// </summary>
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the input data for this impulse.
        /// </summary>
        public string Input { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the output data produced by workflow processing.
        /// </summary>
        public string Output { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets error information if processing failures occur.
        /// </summary>
        public string? Error { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this impulse is being processed in a loop-back scenario.
        /// </summary>
        public bool IsLoopback { get; set; }

        /// <summary>
        /// Gets or sets the timestamp when this impulse was created.
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Gets or sets the timestamp when this impulse was last updated.
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// Gets or sets the processing status of this impulse.
        /// </summary>
        public string Status { get; set; } = "Created";

        /// <summary>
        /// Gets or sets the list of path names that have been traversed.
        /// </summary>
        public List<string> TraversedPathNames { get; set; } = new List<string>();
    }
}
