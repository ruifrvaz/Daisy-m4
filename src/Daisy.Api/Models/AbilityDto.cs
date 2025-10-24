// Copyright (c) 2025 Rui Filipe Rodrigues Vaz
// Licensed under the Apache License, Version 2.0

using System.ComponentModel.DataAnnotations;

namespace Daisy.Api.Models
{
    /// <summary>
    /// Data Transfer Object for Abilities - processing units that transform impulses.
    /// </summary>
    public class AbilityDto
    {
        /// <summary>
        /// Gets or sets the unique identifier for this ability.
        /// </summary>
        [Key]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the display name of the ability.
        /// </summary>
        public string DisplayName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the description of what this ability does.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets the list of paths within this ability.
        /// </summary>
        public List<PathDto> Paths { get; set; } = new List<PathDto>();

        /// <summary>
        /// Gets or sets a value indicating whether this ability is currently enabled.
        /// </summary>
        public bool IsEnabled { get; set; } = true;
    }

    /// <summary>
    /// Data Transfer Object for Paths - execution units within abilities.
    /// </summary>
    public class PathDto
    {
        /// <summary>
        /// Gets or sets the unique name of this path.
        /// </summary>
        public string PathName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the execution priority order for this path.
        /// </summary>
        public int TraverseOrder { get; set; }

        /// <summary>
        /// Gets or sets the description of what this path does.
        /// </summary>
        public string? Description { get; set; }
    }
}
