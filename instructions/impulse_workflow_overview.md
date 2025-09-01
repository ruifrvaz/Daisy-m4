# Impulse Workflow and Modular Architecture Overview

This document outlines the architecture and operational flow of the orchestration engine based on the `Impulse` object and modular plugin structure. It serves as a guide for agents extending or debugging the engine.

---

## Main Concepts

### 1. **Impulse**

`Impulse` is the central data structure that flows through the system. It is:

- Created by a **Receiver**
- Processed by one or more **Abilities** (via `Paths`)
- Consumed by **Transmitters**

### 2. **Core**

`Core` is the central object where the workflow lives. It:
- Runs in parallel with other cores.
- Can communicate with other cores via Pools.

### 3. **Pool**
`Pool` is the collection of module instances that are currently running. They are singletons that:
- Can be used between cores.
- Can be used to communicate between modules.

### 3. **Paths and PathFinder**

The `PathFinder` scans all registered `Paths`:

- Matches based on `TraverseRules`
- Filters previously traversed paths
- Executes qualifying paths in priority order

---

## Workflow Overview: Modules

### 1. **Receivers**

Receivers ingest external input and initialize the `Impulse`.

- Example: `OcrReceiver` populates `Impulse.Input` from an image file path like `FilePath:*`

### 2. **Abilities and Path Traversal**

An **Ability** contains one or more `Paths`, which are dynamically selected and executed based on `TraverseRules`.

Each `Path` contains:

- `TraverseRules`: Determines if the `Impulse` qualifies for this path.
- `HasBeenTraversedRules`: Prevents re-processing of the same path.
- A `TraverseOrder`: Optional order hint for sequential processing.
- Processing logic that enriches or transforms the `Impulse`.

Example:

```csharp
[TraverseRule(PathType = typeof(ImageToTextPath))]
public class ImageToTextTraverseRule : ITraverseRule {
    public bool RuleApplies(Impulse impulse) =>
        impulse.Input.StartsWith("FilePath:");
}
```

### 4. **Transmitters**

Once traversal is complete (no more paths apply), **Transmitters** are triggered to consume the `Impulse.Output`.

Examples:

- `AzureDevOpsTransmitter`: Creates DevOps work items
- `GitTransmitter`: Commits and opens pull requests
- `PipelineRunnerTransmitter`: Executes build and deployment pipelines
- `PipelineRunnerReceiver`: Polls Azure DevOps for pipeline run completion

---

## Startup Initialization

### Startup is managed via specific Factories:

```csharp
var settings = StartupFactory.AppSettingsConfiguration.LoadApplicationSettings();
var serviceProvider = StartupFactory.LoadServices(settings);

AbilityFactory.LoadAbilities(settings, serviceProvider);
TransmitterFactory.LoadTransmitters(settings, serviceProvider);
ReceiverFactory.LoadReceivers(settings, serviceProvider);
```

### Application Settings (appconfig.json)

`AppSettingsConfiguration` loads `ApplicationSettings` from `appconfig.json` and optional secrets.
This configuration file drives which modules and workflows are active:

- `Receivers` and their associated cores
- Lists of `Transmitters`, `Abilities`, and `Workflows`
- `Apis` for external dependencies
- `PathTraverseOrder` for module integration and execution order

Updating `appconfig.json` alters the modules loaded at startup.


### Dependency Injection & Reflection

- All Receivers, Abilities, Paths, and Transmitters are registered via DI.
- `[TraverseRule]` and other attributes allow discovery via reflection.

---

For further extension or debugging, refer to the `Impulse`, `ITraverseRule`, `APath`, and `TransmitterBase` implementations.

