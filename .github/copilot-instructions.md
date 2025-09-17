# Daisy-m4 Workflow Orchestration Engine

**ALWAYS** reference these instructions first and fallback to search or bash commands only when you encounter unexpected information that does not match the info here.

## Working Effectively

### Prerequisites
- .NET 8.0 SDK (confirmed working with 8.0.119)
- Linux/macOS/Windows environment 
- Internet access for package restoration

### Build, Test, and Run Process
1. **Initial Build** (NEVER CANCEL - takes ~47 seconds, allow 90+ minute timeout):
   ```bash
   dotnet build
   ```

2. **Run Tests** (NEVER CANCEL - takes ~13 seconds, allow 30+ minute timeout):
   ```bash
   dotnet test
   ```
   Note: Some factory tests may fail if plugins aren't published yet - this is expected.

3. **Publish Plugins** (REQUIRED before running application):
   ```bash
   ./build-plugins.sh
   ```
   OR manually:
   ```bash
   chmod +x build-plugins.sh
   ./build-plugins.sh
   ```

4. **Run the Application**:
   ```bash
   cd src/Daisy
   dotnet run
   ```

### Code Quality and Formatting
- **Check formatting** (NEVER CANCEL - takes ~16 seconds, allow 30+ minute timeout):
  ```bash
  dotnet format --verify-no-changes
  ```
- **Apply formatting fixes**:
  ```bash
  dotnet format
  ```
- **Check code style**: 
  ```bash
  dotnet format style --verify-no-changes
  ```

## Validation Scenarios

### Essential Manual Testing After Changes
1. **Basic Workflow Engine Test**:
   - Run `dotnet run` from `src/Daisy/` directory
   - Type `start` and press Enter
   - Verify it shows "These are the currently active workflows" and lists "Weather"
   - Type `bye` and press Enter  
   - Verify it outputs "Workflow terminating. Goodbye." and exits

2. **Build and Test Validation**:
   - Always run `dotnet build` and ensure it completes successfully
   - Always run `dotnet test` - most tests should pass (some factory tests may fail due to plugin loading)
   - Always run `./build-plugins.sh` after building to publish plugins
   - Always run `dotnet format --verify-no-changes` to check code formatting

## Architecture Overview

### Core Concepts
- **Impulse**: Central data object containing `.Input`, `.Output`, and `.Error` that flows through workflows
- **Receivers**: Entry points that create Impulses from external input (e.g., Console, WeatherEvent)
- **Abilities**: Processing modules with Paths that enrich/transform Impulses (e.g., Weather API calls, Terminate commands)
- **Transmitters**: Output processors that consume Impulse.Output (e.g., Console output)
- **Workflows**: Collections of modules that define complete processing pipelines
- **Cores**: Parallel execution containers for workflows
- **Paths**: Individual processing units within Abilities with TraverseRules and execution order

### Plugin System
- All modules (Receivers, Abilities, Transmitters, Workflows) are plugins
- Plugins must be published to `src/Daisy/bin/Debug/net8.0/plugins/` before the application can run
- Use `./build-plugins.sh` to automate plugin publishing
- Plugin loading is controlled by `src/Daisy/appconfig.json`

### Key Files and Directories
- `src/Daisy/` - Main application entry point and configuration
- `src/Daisy/appconfig.json` - Runtime configuration for active modules and settings
- `src/Daisy.Resources/` - Core abstracts, interfaces, and shared functionality
- `src/Daisy.Factories/` - Module loading and dependency injection
- `src/Daisy.Abilities/` - Processing modules (Terminate, OutputValidator, Weather, Operator)
- `src/Daisy.Receivers/` - Input modules (Console, WeatherEvent)
- `src/Daisy.Transmitters/` - Output modules (Console)
- `src/Daisy.Workflows/` - Workflow definitions (Starter, Weather)
- `tests/` - Unit tests organized by component
- `instructions/` - Technical documentation (engine_design.md, workflows.md)
- `build-plugins.sh` - Helper script to publish all plugins

## Troubleshooting

### Common Issues
- **"plugins folder not found" error**: Run `./build-plugins.sh` to publish plugins
- **Application fails to start**: Ensure plugins are published and `appconfig.json` is present
- **Tests failing**: Most failures are expected if plugins aren't published; focus on core functionality tests
- **Formatting errors**: Run `dotnet format` to fix whitespace and formatting issues

### Development Workflow
1. Make code changes
2. Run `dotnet build` (NEVER CANCEL - 90+ minute timeout)
3. Run `./build-plugins.sh` if you modified any plugin projects
4. Run `dotnet test` (NEVER CANCEL - 30+ minute timeout)
5. Run `dotnet format` to fix any formatting issues
6. Test manually by running the application and executing validation scenarios
7. Always verify the console workflow responds correctly to `start` and `bye` commands

### Current Workflows
- **Starter**: Console-based workflow that lists available workflows and handles basic commands
- **Weather**: Weather information retrieval workflow (uses wttr.in API)

## Example Development Session

```bash
# Fresh clone setup
dotnet build                          # Takes ~47 seconds, NEVER CANCEL
./build-plugins.sh                    # Publish plugins 
dotnet test                           # Takes ~13 seconds, NEVER CANCEL
dotnet format --verify-no-changes     # Check formatting

# Run and test application
cd src/Daisy
dotnet run
# Type: start [Enter]
# Type: bye [Enter]

# After making changes
cd ../..                              # Back to repo root
dotnet build                          # NEVER CANCEL
./build-plugins.sh                    # Re-publish plugins if needed
dotnet test                           # NEVER CANCEL  
dotnet format                         # Fix formatting
cd src/Daisy && dotnet run            # Test again
```

This orchestration engine enables modular workflow processing where Impulses flow through configurable sequences of Receivers → Abilities → Transmitters, with rule-based path traversal and parallel core execution.