# csharp-background-worker

<hr />

A simple dotnet background work scheduler to run jobs in the background in parallel without blocking the executing thread

### Spec
This library relies on the Generic host's `IHostedService` features to run its workers so make sure that your `IHost` instance is either started or running before attempting to schedule work

the `Multiplexer` interface uses native C Sharp `Channel`s to send data so the implemented methods are thread-safe

to use your own custom routing logic for `IWork` objects imlement your own `Multiplexer` by inheriting the base class and configuring it in your DI

### Future Goals
remove dependency on `IHost` and enable native `Thread` support to run workers