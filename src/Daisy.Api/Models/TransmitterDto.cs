// Copyright (c) 2025 Rui Filipe Rodrigues Vaz
// Licensed under the Apache License, Version 2.0

using System.ComponentModel.DataAnnotations;

namespace Daisy.Api.Models
{
    /// <summary>
    /// Data Transfer Object for Transmitters - output processors that send results to external systems.
    /// </summary>
    public class TransmitterDto
    {
        /// <summary>
        /// Gets or sets the unique identifier for this transmitter.
        /// </summary>
        [Key]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the display name of the transmitter.
        /// </summary>
        public string DisplayName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the description of what this transmitter does.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets the type of transmitter (External, LoopBack).
        /// </summary>
        public string TransmitterType { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether this transmitter is currently enabled.
        /// </summary>
        public bool IsEnabled { get; set; } = true;
    }
}
