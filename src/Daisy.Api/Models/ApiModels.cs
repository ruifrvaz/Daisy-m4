// Copyright (c) 2025 Rui Filipe Rodrigues Vaz
// Licensed under the Apache License, Version 2.0

namespace Daisy.Api.Models
{
    /// <summary>
    /// Request model for triggering a receiver with input data.
    /// </summary>
    public class TriggerReceiverRequest
    {
        /// <summary>
        /// Gets or sets the input data to pass to the receiver.
        /// </summary>
        public string Input { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets additional metadata for the request.
        /// </summary>
        public Dictionary<string, string>? Metadata { get; set; }
    }

    /// <summary>
    /// Request model for executing an ability with an impulse.
    /// </summary>
    public class ExecuteAbilityRequest
    {
        /// <summary>
        /// Gets or sets the impulse ID to process.
        /// </summary>
        public Guid ImpulseId { get; set; }

        /// <summary>
        /// Gets or sets the specific path name to execute (optional).
        /// </summary>
        public string? PathName { get; set; }
    }

    /// <summary>
    /// Request model for transmitting an impulse through a transmitter.
    /// </summary>
    public class TransmitRequest
    {
        /// <summary>
        /// Gets or sets the impulse ID to transmit.
        /// </summary>
        public Guid ImpulseId { get; set; }
    }

    /// <summary>
    /// Response model for workflow control operations.
    /// </summary>
    public class WorkflowControlResponse
    {
        /// <summary>
        /// Gets or sets the workflow name.
        /// </summary>
        public string WorkflowName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the new status of the workflow.
        /// </summary>
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a message describing the result of the operation.
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether the operation was successful.
        /// </summary>
        public bool Success { get; set; }
    }
}
