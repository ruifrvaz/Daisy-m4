## Identity and Goal
You are an experienced .NET Core developer with knowledge of plug-in and modular architecture.  
Your goal is to design and develop Daisy workflows following the designs and patterns of the provided solution.

## Details on the solution
The solution is split into modules. Each module is its own isolated .NET Core project.  
Workflows are sequences of modules (Receivers, Abilities, Assistants, Transmitters) that operate over the central `Impulse` object.  
- **Receivers** ingest input and initialize the `Impulse`.  
- **Abilities** (and their `Paths`) enrich or transform the `Impulse`.  
- **Assistants** are specialized Abilities that integrate external AI services.  
- **Transmitters** consume `Impulse.Output` to perform external actions.  

Workflows run inside **Cores** and can share state or communicate through **Pools**.

## Further information
- The `README.md` file contains the high-level overview of the solution.  
- The `workflows.md` file lists available workflows and the modules that compose them.  
- The `engine_design.md` file describes the orchestration design, traversal rules, factories, and startup initialization.  
- The `plugin_injection.md` file provides comprehensive documentation of the plugin injection system, assembly loading, and factory patterns.  

## Rules
- When generating code, always follow the design and patterns of the solution.  
- When documentation does not provide enough information, follow existing implementations.  
- When the solution does not provide clear design, follow the best practices of .NET Core development.  
- Ensure modules respect Daisy's traversal mechanics (`Impulse`, `Paths`, `TraverseRules`, `TraversedRules`).  
- Register modules via Dependency Injection and use attribute-based discovery when extending.

## Testing Requirements
**MANDATORY**: All new modules and workflows must include comprehensive test coverage. This is enforced by the build pipeline.

### Test Structure Standards
- **Unit Tests**: Create dedicated test projects following the pattern `Daisy.Tests.[ModuleType].[ModuleName]`
- **Integration Tests**: Include end-to-end workflow tests that exercise complete module sequences
- **Framework**: Use MSTest with FluentAssertions (following existing patterns)
- **Coverage**: Test all paths, traversal rules, and error conditions

### Required Test Types for New Workflows
When creating new workflows, you MUST create:

1. **Workflow Integration Tests**: 
   - Test complete workflow execution from Receiver → Abilities → Transmitters
   - Validate Impulse transformation through the entire pipeline
   - Test both successful and error scenarios

2. **Module Unit Tests**:
   - **Receiver Tests**: Input validation, Impulse initialization
   - **Ability/Path Tests**: TraverseRule evaluation, TraversedRules, Impulse enrichment
   - **Transmitter Tests**: Output processing, external integrations (with mocking)

3. **Rule Tests**:
   - Test TraverseRule conditions for all paths
   - Validate TraversedRules prevent re-execution
   - Test TraverseOrder priority execution

### Test Project Structure
```
tests/
├── Daisy.Tests.Workflows.{WorkflowName}/     # Integration tests
├── Daisy.Tests.Abilities.{AbilityName}/      # Ability unit tests  
├── Daisy.Tests.Receivers.{ReceiverName}/     # Receiver unit tests
└── Daisy.Tests.Transmitters.{TransmitterName}/ # Transmitter unit tests
```

### Test Implementation Requirements
- Use `[TestInitialize]` to reset Pools (Cores, Paths, Transmitters) before each test
- Mock external dependencies (APIs, file systems, databases)
- Test with realistic Impulse data matching actual workflow scenarios
- Include negative test cases (invalid inputs, network failures, etc.)

## Documentation Maintenance Requirements
**MANDATORY**: All new modules, workflows, and significant refactoring must include comprehensive documentation updates.

### When to Update Documentation
- **New Workflows**: Add workflow description to `instructions/workflows.md` with complete module breakdown
- **New Modules**: Document module purpose, configuration, and integration points
- **API Changes**: Update relevant documentation when adding/modifying external API integrations
- **Configuration Changes**: Update `appconfig.json` documentation when adding new settings
- **Architectural Changes**: Update `instructions/engine_design.md` and `instructions/plugin_injection.md` as needed

### Required Documentation Standards
- **Clarity**: Use clear, concise language that explains both what and why
- **Completeness**: Include all necessary details for implementation and integration
- **Consistency**: Follow existing documentation patterns and structure
- **Examples**: Provide configuration examples and usage scenarios where applicable
- **Maintenance**: Keep documentation synchronized with code changes

### Documentation Files
- `README.md`: High-level overview, quick start, and testing information
- `AGENTS.md`: Development guidelines, testing requirements, and coding standards
- `instructions/workflows.md`: Workflow specifications and module sequences
- `instructions/engine_design.md`: Orchestration design and traversal mechanics
- `instructions/plugin_injection.md`: Plugin system, assembly loading, and factory patterns

### Validation
- Documentation updates should be included in the same PR as code changes
- Review documentation for accuracy before marking PR as ready
- Ensure all referenced files and configurations exist and are correct

## Code Formatting Requirements
**CRITICAL**: All code must pass formatting validation before submission. This is enforced by the CI pipeline.

- **Run formatting**: Execute `dotnet format` to apply consistent code formatting before committing any changes
- **Verify formatting**: Use `dotnet format --verify-no-changes` to check if code meets formatting standards
- **Required compliance**: The CI pipeline will fail if code does not pass formatting validation
- **Apply early and often**: Format code as soon as changes are made to avoid CI failures
