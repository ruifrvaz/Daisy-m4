# GitHub Workflows Documentation

This directory contains GitHub Actions workflows for automated testing and validation of the Daisy-m4 workflow orchestration engine.

## Workflows Overview

### 🔄 PR Validation (`pr-validation.yml`)
**Trigger:** Pull requests to `main` or `develop` branches

Comprehensive validation pipeline that runs on every pull request to ensure code quality and functionality:

- **Build & Test**: Compiles solution, runs unit tests, and validates plugin deployment
- **Security Scan**: Checks for hardcoded secrets and security anti-patterns
- **Documentation Validation**: Ensures required documentation files are present
- **Code Formatting**: Verifies code follows project formatting standards

**Duration:** ~5-10 minutes  
**Purpose:** Prevent broken code from being merged

### 🚀 Continuous Integration (`ci.yml`)
**Trigger:** Pushes to `main` branch

Full integration testing pipeline for the main branch:

- **Build Validation**: Tests both Debug and Release configurations
- **Integration Tests**: Tests complete workflow functionality
- **Performance Checks**: Basic performance and memory usage validation
- **Plugin Verification**: Comprehensive validation of all 10 expected plugins

**Duration:** ~10-15 minutes  
**Purpose:** Ensure main branch is always in a deployable state

### 🔧 Manual Validation (`manual-validation.yml`)
**Trigger:** Manual dispatch with configurable options

Flexible testing workflow for comprehensive validation:

**Input Options:**
- **Test Level**: `basic` | `comprehensive` | `performance`
- **Environment**: `ubuntu-latest` | `windows-latest` | `macos-latest`
- **Weather API**: Enable/disable actual API testing

**Test Coverage:**
- Cross-platform compatibility testing
- Real weather API integration tests
- Performance benchmarking
- Memory usage analysis

**Duration:** ~5-20 minutes (depending on configuration)  
**Purpose:** Thorough testing before releases or when investigating issues

### 📊 Scenario Testing (`scenario-testing.yml`)
**Trigger:** Daily schedule (2 AM UTC) or manual dispatch

Automated scenario testing to catch regressions:

**Scenario Types:**
- **Basic**: Startup and workflow listing
- **Extended**: Weather workflows and termination
- **Stress**: Rapid startup and memory stress tests
- **Error Handling**: Invalid input processing

**Duration:** ~10-15 minutes  
**Purpose:** Continuous monitoring and regression detection

## Workflow Features

### 🛡️ Safety Features
- **Timeouts**: All jobs have appropriate timeouts to prevent hanging
- **Artifact Upload**: Test results and logs are preserved for analysis
- **Failure Notifications**: Clear error reporting and status indicators
- **Resource Cleanup**: Proper cleanup of resources and temporary files

### 📈 Performance Monitoring
- Plugin deployment verification (expects 10 plugins)
- Startup time measurement
- Memory usage pattern analysis
- Cross-platform compatibility checks

### 🔍 Quality Assurance
- Unit test execution with coverage reporting
- Code formatting validation (`dotnet format`)
- Security scanning for common anti-patterns
- Documentation completeness checks

## Usage Guidelines

### For Pull Requests
1. **PR Validation** runs automatically on all PRs
2. Ensure all checks pass before requesting review
3. Address any formatting issues with `dotnet format`
4. Check uploaded test results if tests fail

### For Manual Testing
1. Go to Actions → Manual Validation
2. Click "Run workflow"
3. Select appropriate test level and environment
4. Enable weather API testing if needed
5. Monitor results and download artifacts

### For Release Preparation
1. Ensure CI pipeline passes on main branch
2. Run Manual Validation with `comprehensive` level
3. Test on multiple platforms if needed
4. Review scenario testing results for any patterns

## Expected Test Results

### Plugin Deployment
The workflows expect exactly **10 plugins** to be deployed:
- `Daisy.Abilities.Operator`
- `Daisy.Abilities.OutputValidator`
- `Daisy.Abilities.Terminate`
- `Daisy.Abilities.Weather`
- `Daisy.Receivers.Console`
- `Daisy.Receivers.WeatherEvent`
- `Daisy.Transmitters.Console`
- `Daisy.Transmitters.WorkflowTrigger`
- `Daisy.Workflows.Starter`
- `Daisy.Workflows.Weather`

### Test Coverage
Current test suite includes:
- **20 tests** in Extensions
- **2 tests** in Operator
- **2 tests** in Receivers  
- **5 tests** in Terminate
- **21 tests** in Transmitters
- **5 tests** in Factories

**Total: 55 unit tests**

### Performance Baselines
- Application startup: Should complete within 30 seconds
- Plugin deployment: All 10 plugins must be present
- Memory usage: No obvious leaks or excessive allocation
- Weather workflow: Should handle mock and real API calls

## Troubleshooting

### Common Issues

1. **Build Failures**
   - Check for compilation errors in job logs
   - Verify all project references are correct
   - Ensure solution file includes all projects

2. **Test Failures**
   - Download test artifacts for detailed analysis
   - Check for environment-specific issues
   - Verify plugin deployment completed

3. **Plugin Deployment Issues**
   - Ensure `DaisyPluginCollector=true` property is set
   - Check Directory.Build.targets configuration
   - Verify all projects build successfully

4. **Performance Issues**
   - Review startup time measurements
   - Check for memory leaks in stress tests
   - Monitor scenario testing results

### Getting Help

1. Check workflow run logs for detailed error messages
2. Download and review uploaded artifacts
3. Run manual validation with comprehensive testing
4. Use scenario testing to isolate specific issues

## Maintenance

### Adding New Tests
1. Follow existing patterns in test projects
2. Update expected test counts in workflow documentation
3. Add new scenarios to scenario testing workflow

### Modifying Workflows
1. Test workflow changes in a branch first
2. Use workflow_dispatch triggers for testing
3. Update documentation when adding new features
4. Consider backward compatibility for existing workflows

### Performance Tuning
1. Monitor workflow execution times
2. Optimize caching strategies for dependencies
3. Adjust timeouts based on actual execution patterns
4. Balance thoroughness with execution speed