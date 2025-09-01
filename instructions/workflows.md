# Workflows

Below are the current available workflows and respective modules. Each workflow is described step-by-step with each respective module listed below each step.


## Daisy.Workflows.Starter

Starter workflow that picks up inputs and triggers other workflows. Can be integrated with an agent (ie. openAI Assistant) to communicate with user, interpret user requests and invoke other workflows.

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

## Daisy.Workflows.TryMe

Weather workflow that given a city name, fetches the weather from a public weather API (pick one). Each module should have appropriate comments regarding its implementation and integration into the workflow.

1. Receive event that is sent from the Starter workflow. The received event has a city name in its impulse input.
	- Daisy.Receivers.TryMeEvent
2. Perform an HTTP GET request on the weather API with the city name. Validate if weather has been received correctly. If not, Emit error to impulse output.
	- Daisy.Abilities.TryMe
3. Return the weather information
	- Daisy.Transmitters.TryMeOutput