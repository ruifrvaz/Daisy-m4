# Workflows

Below are the current available workflows and respective modules. Each workflow is described step-by-step with each respective module listed below each step.

## Daisy.Workflows.Starter

Starter workflow that picks up inputs and triggers other workflows. Can be integrated with an agent (ie. openAI Assistant) to communicate with user, interpret user requests and invoke other workflows.

0. Initialize the workflow core project
    - Daisy.Workflows.Starter
1. Receive external input via System.Console and send Impulse to the PathFinder
	- Daisy.Receivers.Console
2. Trigger other workflows based on input patterns (e.g., "Weather: London")
	- Daisy.Abilities.Operator
3. Terminate the workflow if impulse.input starts with bye (optional)
	- Daisy.Abilities.Terminate
4. Determine if an impulse.input is valid by checking if output is empty (runs last)
	- Daisy.Abilities.OutputValidator
5. Transmit impulse.output via System.Console
    - Daisy.Transmitters.Console
6. Trigger workflows via loopback mechanism when workflows are invoked
    - Daisy.Transmitters.WorkflowTrigger

## Daisy.Workflows.Weather

Weather workflow that given a city name, fetches the weather from a public weather API (pick one). Each module should have appropriate comments regarding its implementation and integration into the workflow.

0. Initialize the workflow core project
    - Daisy.Workflows.Weather
1. Receive event that is sent from the Starter workflow. The received event has a city name in its impulse input. Generate a new chain with impulse.AddChain("cityName: {cityName}") extension method.
    - Daisy.Receivers.WeatherEvent
2. Perform an HTTP GET request on the weather API with cityName as parameter. Validate if the weather response has been received correctly. If not, Emit error to impulse output. 
   Place HttpClient inside a WeatherService. PathTraverseOrder is 50. Can traverse when input chain contains "cityName: {cityName}"
	- Daisy.Abilities.Weather
3. Return the weather information via console output
	- Daisy.Transmitters.Console

## Daisy.Workflows.Flights

Flights workflow that given a city name, fetches available flights to that city from a flights API. Each module has appropriate comments regarding its implementation and integration into the workflow.

0. Initialize the workflow core project
    - Daisy.Workflows.Flights
1. Receive event that is sent from the Starter workflow. The received event has a city name in its impulse input. Generate a new chain with impulse.AddChain("flights: {cityName}") extension method.
    - Daisy.Receivers.FlightsEvent
2. Perform a flight search for the specified city using AviationStack API. Falls back to mock data if API key is not configured. PathTraverseOrder is 50. Can traverse when input chain contains "flights: {cityName}"
	- Daisy.Abilities.Flights
3. Return the flight information via console output
	- Daisy.Transmitters.Console

## Daisy.Abilities.CopilotSdk

Copilot SDK ability that processes AI prompts using the GitHub Copilot SDK. This ability enables AI-powered agentic workflows by integrating Copilot's production-tested agent runtime for planning, tool invocation, and AI interactions.

**Integration with Workflows:**

The CopilotSdk ability can be used in any workflow by adding a "copilot:" chain to the impulse. It processes the prompt through the GitHub Copilot SDK and returns AI-generated responses.

**Prerequisites:**
- GitHub Copilot CLI must be installed and authenticated ([Installation guide](https://docs.github.com/en/copilot/how-tos/set-up/install-copilot-cli))
- GitHub Copilot subscription is required

**Configuration (appconfig.json):**
```json
"CopilotSdk": {
  "Model": "gpt-4.1",
  "Streaming": true,
  "TimeoutSeconds": 30
}
```

**Usage Pattern:**
- PathTraverseOrder: 60
- Can traverse when input chain contains "copilot: {prompt}"
- Adds response to output chain as "Copilot: {response}"

**Example:**
```csharp
impulse.AddChain("copilot: Write a hello world program in C#");
// After traversal:
// impulse.Output contains "Copilot: Here is a hello world program..."
```

**Features:**
- Streaming responses for real-time feedback
- Configurable AI model selection
- Graceful fallback when Copilot CLI is not available
- Timeout protection for long-running requests