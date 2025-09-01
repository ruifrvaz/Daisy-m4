# Daisy-m4
General-purpose workflow orchestration engine

-----------------------------------------------------

## High-Level System Architecture: Input-Process-Output Iterations
Modular solution where **Impulses** flow from **Receivers** through rule-based **Abilities** to **Transmitters**, guided by centralized factories and a path-finding helper.

### Outer Loop: Receivers (1-N) entry point(s) to the system. Populate a single Impulse object.
- Trigger Transmitters directly (end cycle), or
- Enter the Ability loop (inner loop) for processing.

### Inner Loop: Ability Execution (Micro Iteration)
Abilities (1-N): Process or mutate the Impulse.
- Can loop over multiple abilities before sending control to a Transmitter (exit point for this impulse).

### Exit point(s) or loopback recursion: Transmitters (1-N) output the Impulse object.
- Output processors (DevOps work items, Git PRs, pipelines).
- Can optionally trigger new Receivers (updating the impulse) via a loopback mechanism, restarting the whole macro-cycle.

### Impulse:
Central object carrying data and metadata. 
It is the sole object moving through receivers, abilities, and transmitters, accumulating state and history as it travels.

It evolves state as it passes through Receivers, Abilities, and Transmitters.

As it passes through each connection, it chains inputs and outputs, which allows it to carry history and data throughout each stage.

## Key features

- Infinite Turing-like iteration.
- Statically select abilities per iteration.
- Evaluate exit conditions after each iteration.
- Deterministic halting via Transmitter or Ability rules.
- Run multiple workflows in parallel.
- Communicate between workflows.
- Recursion or chaining across multiple receivers/abilities/transmitters.
- External stimulus (e.g., webhooks, CI/CD) to kick off new Receivers.

## License
Apache License 2.0 © 2025 Rui Filipe Rodrigues Vaz.  
See [LICENSE](./LICENSE) and [NOTICE](./NOTICE) for details.