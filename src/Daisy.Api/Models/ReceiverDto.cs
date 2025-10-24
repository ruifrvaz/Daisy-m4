// Copyright (c) 2025 Rui Filipe Rodrigues Vaz
// Licensed under the Apache License, Version 2.0

using System.ComponentModel.DataAnnotations;

namespace Daisy.Api.Models
{
    /// <summary>
    /// Data Transfer Object for Receivers - entry points that ingest external input.
    /// </summary>
    public class ReceiverDto
    {
        /// <summary>
        /// Gets or sets the unique identifier for this receiver.
        /// </summary>
        [Key]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the display name of the receiver.
        /// </summary>
        public string DisplayName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the description of what this receiver does.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets the type of receiver (External, LoopBack, Event).
        /// </summary>
        public string ReceiverType { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the list of core namespaces where this receiver is active.
        /// </summary>
        public List<string> RunOnCores { get; set; } = new List<string>();

        /// <summary>
        /// Gets or sets a value indicating whether this receiver is currently enabled.
        /// </summary>
        public bool IsEnabled { get; set; } = true;
    }
}
