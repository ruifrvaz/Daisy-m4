# Workflows

Below are the current available workflows and respective modules. Each workflow is described step-by-step with each respective module listed below each step.

## Daisy.Workflows.Starter

Starter workflow that picks up inputs and triggers other workflows. Can be integrated with an agent (ie. openAI Assistant) to communicate with user, interpret user requests and invoke other workflows.

0. Initialize the workflow core project
    - Daisy.Workflows.Starter
1. Receive external input via System.Console and send Impulse to the PathFinder
	- Daisy.Receivers.Console
2. List available workflows when impulse.input starts with start (optional)
	- Daisy.Abilities.ConsoleStart
3. Terminate the workflow if impulse.input starts with bye (optional)
	- Daisy.Abilities.Terminate
4. Transmit impulse.output via System.Console
    - Daisy.Transmitters.Console
5. Determine if an impulse.input is valid by checking if output is empty (runs last)
	- Daisy.Abilities.OutputValidator

## Daisy.Workflows.Weather

Weather workflow that given a city name, fetches the weather from a public weather API (pick one). Each module should have appropriate comments regarding its implementation and integration into the workflow.

0. Initialize the workflow core project
    - Daisy.Workflows.Weather
1. Receive event that is sent from the Starter workflow. The received event has a city name in its impulse input. Generate a new chain with impulse.AddChain("cityName: {cityName}") extension method.
    - Daisy.Receivers.WeatherEvent
2. Perform an HTTP GET request on the weather API with cityName as parameter. Validate if the weather response has been received correctly. If not, Emit error to impulse output. 
   Place HttpClient inside a WeatherService. PathTraverseOrder is 50. Can traverse when input chain contains "cityName: {cityName}"
	- Daisy.Abilities.Weather
3. Return the weather information
	- Daisy.Transmitters.WeatherOutput