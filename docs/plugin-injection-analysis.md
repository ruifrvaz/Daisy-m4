# Plugin Injection Mechanism Analysis

## Overview

The Daisy-m4 orchestration engine uses a complex runtime plugin injection system based on .NET's `AssemblyLoadContext` to dynamically load workflow modules (Receivers, Abilities, Transmitters, and Workflows). While this provides modularity and flexibility, it introduces significant technical debt and maintenance challenges.

## Current Plugin Injection Architecture

### High-Level Flow

1. **Build-Time Plugin Publishing**: The `Directory.Build.targets` MSBuild script automatically publishes all plugin assemblies to individual folders under `/plugins` during build
2. **Runtime Assembly Discovery**: `AssemblyPluginsLoader` scans the plugins directory and loads assemblies based on configuration from `appconfig.json`
3. **Type Resolution**: Factory classes use reflection to discover types implementing specific interfaces
4. **Instance Creation**: Dynamic instantiation with `Activator.CreateInstance()` followed by interface casting
5. **Pool Registration**: Successfully cast instances are added to singleton pools for runtime use

### Core Components

#### 1. AssemblyPluginsLoader (`src/Daisy.Resources/Startup/AssemblyPluginsLoader.cs`)

The central class managing plugin assembly loading, inheriting from `AssemblyLoadContext`:

```csharp
public class AssemblyPluginsLoader : AssemblyLoadContext
{
    private readonly AssemblyDependencyResolver _resolver;
    private static readonly List<AssemblyPluginsLoader> _pluginContexts = new();
    
    public AssemblyPluginsLoader(string pluginMainAssemblyPath) 
        : base(isCollectible: true)
}
```

**Key Responsibilities:**
- Creates isolated assembly load contexts for each plugin
- Attempts to maintain type identity by preferring assemblies from the default context
- Handles dependency resolution for plugin assemblies
- Provides fallback loading from plugin-specific contexts when types aren't in default context

**Critical Logic:**
```csharp
// Check if assembly is already loaded in the default context
var existingAssembly = AppDomain.CurrentDomain.GetAssemblies()
    .FirstOrDefault(a => AssemblyName.ReferenceMatchesDefinition(assemblyName, a.GetName()));

if (existingAssembly != null) {
    // Use the existing assembly from the default context to maintain type identity
    yield return existingAssembly;
} else {
    // Load in plugin context only if not already available in default context
    var alc = new AssemblyPluginsLoader(mainDll);
    _pluginContexts.Add(alc);  // keep context alive
    yield return alc.LoadFromAssemblyPath(mainDll);
}
```

#### 2. Factory Pattern Implementation

Five factory classes handle different plugin types:

- **`StartupFactory`**: Loads `IDaisyService` implementations and configures dependency injection
- **`AbilityFactory`**: Loads `IPath` implementations (processing logic) with traverse rules
- **`ReceiverFactory`**: Loads input receivers (`IExternalReceiver`, `ILoopBackReceiver`, `IEventReceiver`)
- **`TransmitterFactory`**: Loads output transmitters (`IExternalTransmitter`, `ILoopBackTransmitter`)
- **`CoreFactory`**: Loads workflow cores (`ICore` implementations)

**Common Pattern:**
```csharp
var pluginsRoot = Path.Combine(AppContext.BaseDirectory, "plugins");
var assemblies = AssemblyPluginsLoader.LoadFromPluginsFolder(pluginsRoot, configuredNames);

foreach (var assembly in assemblies) {
    var types = assembly.GetTypes().Where(type => targetInterface.IsAssignableFrom(type) && type.IsClass);
    foreach (var type in types) {
        dynamic instance = Activator.CreateInstance(type, parameters);
        var typedInstance = instance as ITargetInterface;
        if (typedInstance != null) {
            Pool.Add(typedInstance);
        }
    }
}
```

#### 3. Build System Integration (`Directory.Build.targets`)

MSBuild automatically publishes plugins during build:

```xml
<Target Name="PublishPlugins" AfterTargets="Build" 
        Condition="'$(DaisyPluginCollector)' == 'true'">
  <ItemGroup>
    <PluginProjects Include="$(RepoRoot)**/Daisy.Abilities.*/*.csproj" />
    <PluginProjects Include="$(RepoRoot)**/Daisy.Receivers.*/*.csproj" />
    <PluginProjects Include="$(RepoRoot)**/Daisy.Transmitters.*/*.csproj" />
    <PluginProjects Include="$(RepoRoot)**/Daisy.Workflows.*/*.csproj" />
  </ItemGroup>
  
  <MSBuild Projects="@(PluginProjects)" Targets="Publish"
           Properties="PublishDir=$(TargetDir)plugins\%(PluginProjects.Filename)\" />
</Target>
```

This creates a structure like:
```
plugins/
├── Daisy.Abilities.Weather/
│   ├── Daisy.Abilities.Weather.dll
│   ├── Daisy.Resources.dll (duplicated)
│   └── [20+ Microsoft.Extensions.*.dll files]
├── Daisy.Abilities.Terminate/
│   ├── Daisy.Abilities.Terminate.dll  
│   ├── Daisy.Resources.dll (duplicated)
│   └── [same 20+ Microsoft.Extensions.*.dll files]
└── [8 more plugin directories with same duplication]
```

## Pain Points and Technical Debt

### 1. **Massive Dependency Duplication**

**Problem**: Each plugin folder contains a complete copy of all dependencies (~1.1MB per plugin, 11MB total for 10 plugins)

**Evidence**: `Daisy.Resources.dll` is duplicated 10 times, along with 20+ Microsoft.Extensions assemblies in each folder.

**Impact**: 
- Bloated deployment size (90% redundancy)
- Slower builds due to repeated publishing
- Increased storage and transfer costs
- Complex dependency management

### 2. **Type Identity Issues**

**Problem**: Same types loaded from different `AssemblyLoadContext` instances are not considered identical by .NET's type system.

**Evidence**: The `AssemblyPluginsLoader.Load()` method explicitly tries to avoid this:
```csharp
try {
    // Always prefer assemblies already known to the host to keep type identity consistent
    return Default.LoadFromAssemblyName(assemblyName);
} catch (FileNotFoundException) {
    var path = _resolver.ResolveAssemblyToPath(assemblyName);
    return path is null ? null : LoadFromAssemblyPath(path);
}
```

**Impact**:
- Potential runtime cast failures when types from different contexts are compared
- Difficult-to-debug issues where `obj is ISomeInterface` returns false even when it should be true
- Inconsistent behavior depending on assembly loading order

### 3. **Fragile Dynamic Casting Pattern**

**Problem**: Heavy reliance on `dynamic` types and `as` operator casting without proper error handling.

**Evidence**: Pattern repeated throughout factories:
```csharp
dynamic receiverObject = Activator.CreateInstance(receiverType, parameters);
var receiver = receiverObject as IExternalReceiver;
Resources.Pools.ExternalReceivers.Instance.Pool.Add(receiver);
```

**Impact**:
- Silent failures when casts return null (modules simply don't load)
- No logging or diagnostics when plugins fail to load
- Runtime exceptions are possible but may be swallowed

### 4. **Exception Handling Complexity**

**Problem**: Special handling required for reflection exceptions.

**Evidence**: `SafeGetExportedTypes()` method handles `ReflectionTypeLoadException`:
```csharp
public static Type[] SafeGetExportedTypes(Assembly asm) {
    try { 
        return asm.GetExportedTypes(); 
    } catch (ReflectionTypeLoadException ex) { 
        return ex.Types!.Where(t => t is not null)!.ToArray()!; 
    }
}
```

**Impact**:
- Masks underlying issues with plugin assemblies
- Difficult to diagnose why specific types aren't being loaded
- Potential for partial loading scenarios

### 5. **Configuration-Assembly Coupling**

**Problem**: Plugin loading is tightly coupled to configuration structure in `appconfig.json`.

**Evidence**: Assembly names must match configuration keys exactly:
```json
"Receivers": {
  "Daisy.Receivers.Console": { "RunOnCores": ["Daisy.Workflows.Starter"] },
  "Daisy.Receivers.WeatherEvent": { "RunOnCores": ["Daisy.Workflows.Weather"] }
},
"Abilities": [ "Daisy.Abilities.Operator", "Daisy.Abilities.Terminate" ]
```

**Impact**:
- Brittle configuration where typos cause silent failures
- Difficult to rename or reorganize plugins
- No validation between configuration and available assemblies

### 6. **Memory Management Concerns**

**Problem**: Plugin contexts are kept alive indefinitely.

**Evidence**: `_pluginContexts.Add(alc);  // keep context alive`

**Impact**:
- Memory leaks as plugins can never be unloaded
- No support for plugin hot-swapping or updates
- Resource accumulation over time

### 7. **Testing Complexity**

**Problem**: Plugin loading logic duplicated in test scenarios.

**Evidence**: Test classes like `ReceiverFactoryTest` replicate the same assembly loading logic:
```csharp
var receiverAssemblies = AssemblyPluginsLoader.LoadFromPluginsFolder(pluginsRoot, Settings!.Receivers.Keys);
```

**Impact**:
- Test maintenance burden
- Potential for tests and production to diverge
- Difficult to isolate specific plugin loading scenarios

## Concrete Examples of Problems

### 1. Silent Plugin Loading Failures

In `TransmitterFactory.LoadExternalTransmitters()`:
```csharp
dynamic transmitterObject = Activator.CreateInstance(transmitterType);
var transmitter = transmitterObject as IExternalTransmitter;
ExternalTransmitters.Instance.Pool.Add(transmitter);
```

If the cast fails, `transmitter` becomes `null` and gets added to the pool, potentially causing null reference exceptions later.

### 2. Directory Structure Dependence  

The system fails completely if the `plugins` folder doesn't exist:
```csharp
var assemblies = Directory.Exists(pluginsRoot)
    ? AssemblyPluginsLoader.LoadFromPluginsFolder(pluginsRoot, settings.Abilities).ToList()
    : throw new Exception("Error loading modules: plugins folder not found.");
```

### 3. Assembly Name Matching Issues

Complex logic in `AssemblyName.ReferenceMatchesDefinition()` can fail with version mismatches or different build configurations, leading to:
- Same assembly loaded in multiple contexts
- Type identity problems between different contexts
- Unpredictable behavior depending on build order

### 4. Reflection Exception Masking

The `SafeGetExportedTypes()` method silently filters out problematic types:
```csharp
catch (ReflectionTypeLoadException ex) { 
    return ex.Types!.Where(t => t is not null)!.ToArray()!; 
}
```

This can hide real problems with plugin assemblies, making debugging very difficult.

## Recommendations for Improvement

### Immediate Actions
1. **Add comprehensive logging** to all plugin loading operations
2. **Implement validation** for configuration vs. available assemblies
3. **Add null checks** after dynamic casting operations
4. **Create plugin loading health checks** for startup diagnostics

### Medium-term Solutions
1. **Consolidate shared dependencies** to reduce duplication
2. **Implement proper error handling** with specific exception types
3. **Add plugin loading metrics** and monitoring
4. **Create plugin manifest system** to replace configuration coupling

### Long-term Architectural Changes
1. **Move to compile-time plugin discovery** to eliminate runtime assembly loading
2. **Implement proper dependency injection** container-based plugin system
3. **Add support for plugin versioning** and compatibility checking
4. **Design plugin isolation** boundaries for better testability and maintenance

This analysis provides a foundation for addressing the technical debt in Daisy-m4's plugin injection system and moving toward a more maintainable, reliable, and performant architecture.