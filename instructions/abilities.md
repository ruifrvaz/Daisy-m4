# Abilities

This document describes the available abilities in the Daisy-m4 system. Abilities are processing modules that enrich or transform the `Impulse` as it flows through workflows.

Each ability contains one or more `Paths` that include:
- **TraverseRules**: Conditions for execution
- **TraversedRules**: Prevents re-processing
- **TraverseOrder**: Optional execution priority
- Processing logic to mutate or enrich the `Impulse`

---

## Daisy.Abilities.CopilotSdk

Copilot SDK ability that processes AI prompts using the GitHub Copilot SDK. This ability enables AI-powered agentic workflows by integrating Copilot's production-tested agent runtime for planning, tool invocation, and AI interactions.

### Purpose

Provides AI-powered prompt processing capabilities to Daisy workflows through integration with the GitHub Copilot SDK. Enables natural language interaction and code generation within workflow pipelines.

### Prerequisites

- GitHub Copilot CLI must be installed and authenticated ([Installation guide](https://docs.github.com/en/copilot/how-tos/set-up/install-copilot-cli))
- GitHub Copilot subscription is required

### Configuration

Add the following to `appconfig.json`:

```json
"Apis": {
  "CopilotSdk": {
    "Model": "gpt-4.1",
    "Streaming": true,
    "TimeoutSeconds": 30
  }
}
```

**Configuration Options:**
- `Model`: AI model to use (e.g., "gpt-4.1", "gpt-4o")
- `Streaming`: Enable streaming responses for real-time feedback (default: true)
- `TimeoutSeconds`: Request timeout in seconds (default: 30, must be positive)

### Integration Points

**Traverse Rules:**
- **PathTraverseOrder**: 60
- **CanTraverse**: When input chain contains `"copilot: {prompt}"`
- **Traversed**: When output chain contains `"Copilot: {response}"`

**Service Registration:**
- `CopilotSdkService` implements `ICopilotSdkService` and `IAsyncDisposable`
- Registered via `IDaisyService` interface
- Managed through `ServiceContainer.Instance`

**Impulse Chain Pattern:**
```csharp
// Input: Add copilot chain to trigger the ability
impulse.AddChain("copilot: Write a hello world program in C#");

// Output: Response added to output chain
// impulse.Output contains "Copilot: Here is a hello world program..."
```

### Usage Example

```csharp
// In a receiver or another ability
var impulse = new Impulse();
impulse.AddChain("copilot: Explain dependency injection in .NET");

// The CopilotSdkPath will:
// 1. Detect the "copilot:" prefix in the input chain
// 2. Extract the prompt
// 3. Send it to the Copilot SDK
// 4. Add the AI response to the output chain
// 5. Emit the updated impulse
```

### Features

- **Streaming Responses**: Real-time AI responses using streaming events
- **Configurable Models**: Support for different GPT models
- **Graceful Fallback**: Informative messages when Copilot CLI is unavailable
- **Timeout Protection**: Proper cancellation with `CancellationToken`
- **Thread Safety**: Lock-based synchronization for event handling
- **Resource Management**: Proper disposal with `IAsyncDisposable`

### Error Handling

The ability gracefully handles several error scenarios:

1. **CLI Not Available**: Returns informative message if Copilot CLI is not installed or authenticated
2. **Timeout**: Returns timeout message after configured timeout period
3. **SDK Errors**: Catches and returns SDK-specific errors with context
4. **Empty Prompt**: Returns error message when prompt is missing or whitespace-only

### Dependencies

- `GitHub.Copilot.SDK` v0.1.10 (NuGet package)
- GitHub Copilot CLI (external dependency)

---

## Future Abilities

Additional abilities should be documented here following the same structure:
- Purpose
- Prerequisites
- Configuration
- Integration Points
- Usage Example
- Features
- Error Handling
- Dependencies
