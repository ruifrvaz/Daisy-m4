# Daisy-m4 Workflow Orchestration Engine

Always reference these instructions first and fallback to search or bash commands only when you encounter unexpected information that does not match the info here.

## Working Effectively

### Bootstrap, Build, and Test
- Restore dependencies: `dotnet restore` (takes ~2-30 seconds depending on cache state)
- Build the solution: `dotnet build` (takes ~15-23 seconds, NEVER CANCEL - Set timeout to 60+ minutes for safety)
- Run tests: `dotnet test` (takes ~22 seconds, NEVER CANCEL - Set timeout to 60+ minutes for safety)
- All tests should pass (29 total: 20 in Extensions, 2 in Receivers, 5 in Factories, 2 in Operator)
- Format code: `dotnet format` (required before committing due to strict whitespace rules)

### Run the Application
- Start the main application: `cd src/Daisy && dotnet run` 
- The application runs interactively and displays a menu with options
- Commands are processed individually (one per line):
  - Type `start` to list available workflows
  - Type `Weather: [city name]` to trigger weather workflow (e.g., "Weather: London")
  - Type `bye` to terminate the application
- Weather API endpoint: https://wttr.in/ (configured in `appconfig.json`)
- Application produces one nullable reference warning during startup (expected behavior)

## Validation

### Manual Testing Scenarios
- ALWAYS manually validate workflow functionality after making changes to core components
- Test the complete user workflow:
  1. Run `cd src/Daisy && dotnet run`
  2. Application displays menu with options to type "start", "bye", or workflow names
  3. Type individual commands (each on a new line) to test functionality
  4. Type `bye` to terminate gracefully
- ALWAYS run `dotnet format` to fix code formatting before committing (required due to strict whitespace formatting rules)
- ALWAYS use `dotnet format --verify-no-changes` to validate formatting compliance before submitting
- ALWAYS run complete build and test cycle before publishing changes

**CRITICAL**: Code formatting failures will cause CI pipeline failures. The PR validation workflow runs `dotnet format --verify-no-changes` and will reject improperly formatted code.

### Build Validation
- Solution builds with warnings only (nullable reference type warnings are expected)
- Plugins are automatically copied to `src/Daisy/bin/Debug/net8.0/plugins/` during build
- Check that all 10 plugin folders exist: Abilities (4), Receivers (2), Transmitters (2), Workflows (2)

## Architecture Overview

### Core Concepts
- **Impulse**: Central data object carrying state through the workflow (.Input, .Output, .Error)
- **Receivers**: Entry points that initialize Impulse objects (Console, WeatherEvent)
- **Abilities**: Processing modules that enrich/transform Impulses via Paths (Weather, Terminate, OutputValidator, Operator)
- **Transmitters**: Output processors that consume Impulse.Output (Console, WorkflowTrigger)
- **Cores**: Workflow containers that run in parallel
- **Pools**: Singleton collections for inter-core communication
- **Paths**: Execution units within Abilities with traverse rules and ordering

### Plugin Architecture
- All modules are loaded dynamically from the `plugins/` directory at runtime
- Modules are registered via Dependency Injection with reflection-based discovery
- Configuration in `appconfig.json` controls which modules are active and their execution order

## Key Files and Locations

### Configuration
- `src/Daisy/appconfig.json` - Main application configuration (APIs, module settings, execution order)
- `tests/Daisy.Tests.Factories/appconfig.json` - Test configuration

### Core Projects
- `src/Daisy/` - Main executable application
- `src/Daisy.Resources/` - Shared interfaces, models, abstracts, pools
- `src/Daisy.Factories/` - Module loading and registration logic

### Workflow Projects
- `src/Daisy.Workflows/Daisy.Workflows.Starter/` - Main workflow entry point
- `src/Daisy.Workflows/Daisy.Workflows.TryMe/` - Weather workflow implementation

### Module Projects
- `src/Daisy.Abilities/` - Processing modules (Weather, Terminate, OutputValidator, Operator)
- `src/Daisy.Receivers/` - Input handlers (Console, WeatherEvent)  
- `src/Daisy.Transmitters/` - Output handlers (Console, WorkflowTrigger)

### Documentation
- `README.md` - High-level system architecture and features
- `instructions/engine_design.md` - Technical design, startup initialization, extension guidance
- `instructions/workflows.md` - Available workflows and their module composition
- `agents.md` - Guidelines for AI agents working with the codebase

## Build System

### MSBuild Configuration
- `Directory.Build.targets` - Plugin publishing configuration
- `DaisyPluginCollector=true` property triggers automatic plugin deployment
- Plugins are published to individual folders under `/plugins` during build

### Dependencies
- .NET 8.0 target framework
- Microsoft.Extensions.* packages for DI and configuration
- FluentAssertions for testing
- HttpClient for weather API integration

## Common Patterns and Rules

### Module Development
- All modules implement specific interfaces (IPath, IReceiver, ITransmitter, etc.)
- Use `[TraverseRule]` attributes for path discovery and execution control
- `TraverseOrder` property controls execution priority (lower numbers execute first)
- `HasBeenTraversedRules` prevent re-processing of the same path

### Code Quality
- Build produces nullable reference type warnings - these are expected
- Use `dotnet format` for consistent code formatting
- Follow existing patterns when extending functionality
- Respect the modular plugin architecture

### Configuration Management
- All runtime behavior is controlled via `appconfig.json`
- API endpoints and credentials are configured in the "Apis" section
- Module activation is controlled by the respective arrays (Receivers, Abilities, etc.)
- Path execution order is defined in "PathTraverseOrder" section

## Troubleshooting

### Common Issues
- Missing plugins folder: Run `dotnet build` to regenerate plugin deployments
- Module not loading: Check `appconfig.json` configuration for module inclusion
- Test failures: Ensure all dependencies are restored with `dotnet restore`
- Weather workflow not working: Verify internet connectivity and API configuration

### Build Artifacts Location
- Main executable: `src/Daisy/bin/Debug/net8.0/Daisy.dll`
- Plugins: `src/Daisy/bin/Debug/net8.0/plugins/[ModuleName]/`
- Test binaries: `tests/[TestProject]/bin/Debug/net8.0/`