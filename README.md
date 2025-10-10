# Daisy-m4
General-purpose workflow orchestration engine. 

Supports:
- Modular, pluggable logic
- Rule-driven traversal
- Hand-off to external systems

-----------------------------------------------------

## High-Level System Architecture: Input-Process-Output Iterations
Modular solution where **Impulses** flow from **Receivers** through rule-based **Abilities** to **Transmitters**, guided by centralized factories and a path-finding helper.

### Impulse:
Central object carrying data and metadata through receivers, abilities, and transmitters. It:
- Accumulates state and history as it travels.
- Chains inputs and outputs.
- Carries history and data throughout each module.

### Outer Loop: Receivers (1-N) entry point(s) to the system. Populate a single Impulse object.
- Trigger Transmitters directly (end cycle), or
- Enter the Ability loop (inner loop) for processing.

### Inner Loop: Ability Execution (Micro Iteration)
Abilities (1-N): Process or mutate the Impulse.
- Can loop over multiple abilities before sending control to a Transmitter (exit point for this impulse).

### Exit point(s) or loopback recursion: Transmitters (1-N) output the Impulse object.
- Output processors (DevOps work items, Git PRs, pipelines).
- Can optionally trigger new Receivers (updating the impulse) via a loopback mechanism, restarting the whole macro-cycle.

## Key features
- Infinite Turing-like iteration.
- Statically select abilities per iteration.
- Evaluate exit conditions after each iteration.
- Deterministic halting via Ability rules.
- Run multiple workflows in parallel.
- Communicate between workflows.
- Recursion or chaining across multiple receivers/abilities/transmitters.
- External stimulus (e.g., webhooks, CI/CD) to kick off new Receivers.

## How to create your workflows using prompt-driven development (PDD):
- Add your workflow to the workflows.md file.
- Instruct your coding agent of choice to analyze the documentation and implement the workflow, step-by-step.

## Testing and Validation

This project includes comprehensive automated testing and validation workflows:

- **PR Validation**: Automated checks on every pull request (build, test, formatting, security)
- **Continuous Integration**: Full integration testing on main branch commits
- **Manual Validation**: Flexible testing with configurable options and cross-platform support
- **Scenario Testing**: Daily automated testing of various usage scenarios

### Quick Start Testing
```bash
# Restore dependencies and build
dotnet restore
dotnet build

# Run all tests
dotnet test

# Run the application
cd src/Daisy
dotnet run
```

### Code Formatting Requirements
**All contributors and coding agents must ensure code meets formatting standards:**

- **Required**: Run `dotnet format` before committing changes
- **Validation**: Use `dotnet format --verify-no-changes` to verify compliance
- **CI Enforcement**: Pull requests will fail if formatting requirements are not met
- **Best Practice**: Apply formatting early and often during development

```bash
# Recommended workflow
dotnet format                    # Apply formatting
dotnet format --verify-no-changes  # Verify formatting
```

For detailed testing information, see [.github/workflows/README.md](.github/workflows/README.md).

## License
Apache License 2.0 © 2025 Rui Filipe Rodrigues Vaz.  
See [LICENSE](./LICENSE) and [NOTICE](./NOTICE) for details.