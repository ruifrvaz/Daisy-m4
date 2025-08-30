# Workflows

Below are the current available workflows and respective modules. Each workflow is described step-by-step with each respective module listed below each step.


## Daisy.Workflows.Starter

Starter workflow that picks up inputs and triggers other workflows. Can be integrated with a chat service (ie. openAI Assistant) to communicate with user and interpret user requests.

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