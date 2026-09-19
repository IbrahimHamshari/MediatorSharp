# MediatorSharp

MediatorSharp is a lightweight, extensible mediator library for .NET 8, designed to decouple request handling and enable pipeline behaviors in your applications. This package (`MediatorSharp`) is the core library; the repository also ships a sample ASP.NET Core Web API project and an xUnit test suite.

All public types live in the flat `MediatorSharp` namespace:

```csharp
using MediatorSharp;
```

## Features

- **Request/Response Mediation**: Send requests and receive responses via strongly-typed handlers, synchronously or asynchronously.
- **Pipeline Behaviors**: Add cross-cutting concerns (logging, validation, etc.) using pipeline behaviors.
- **Automatic Discovery**: Handlers and pipelines are automatically discovered and registered via reflection.
- **Integration with Dependency Injection**: Built on top of Microsoft.Extensions.DependencyInjection.

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)

### Registering Mediator and Pipelines

In `Program.cs`:

```csharp
using MediatorSharp;
using System.Reflection;

builder.Services.AddMediatorAndDiscoverRequestsWithPipelines([Assembly.GetExecutingAssembly()]);
```

This registers all request handlers and pipeline behaviors found in the current assembly. Use `AddMediatorAndDiscoverRequests` if you want handler discovery without registering pipelines.

### Defining Requests, Handlers, and Pipelines

- **Request**: Implement `IRequest` or `IRequest<TResponse>`.
- **Handler**: Implement `IRequestHandler<TRequest>`, `IRequestHandler<TRequest, TResponse>`, `IAsyncRequestHandler<TRequest>`, or `IAsyncRequestHandler<TRequest, TResponse>`.
- **Pipeline**: Implement `IPipelineBehavior<TRequest>`, `IPipelineBehavior<TRequest, TResponse>`, `IAsyncPipelineBehavior<TRequest>`, or `IAsyncPipelineBehavior<TRequest, TResponse>`.

Pipeline behaviors wrap the handler. The first-registered behavior is outermost and runs first; registration order is the contract.

### Sending Requests

```csharp
using MediatorSharp;

Result<Test> result = mediator.Send(new TestRequest(1));
Result simple = mediator.Send(new Test2Request(1));

Result<Test> asyncResult = await mediator.SendAsync(new TestRequest(1));
```

A `Result` reports success or failure through `IsSuccess` and carries any `Errors`. A `Result<T>` additionally exposes `Value` on success.

## Installing

This package is published to GitHub Packages. Add the feed and authenticate before restoring:

```bash
dotnet nuget add source --username IbrahimHamshari --password YOUR_GITHUB_PAT --store-password-in-clear-text --name github "https://nuget.pkg.github.com/IbrahimHamshari/index.json"
dotnet add package MediatorSharp
```

## Dependencies

- [Microsoft.Extensions.DependencyInjection.Abstractions](https://www.nuget.org/packages/Microsoft.Extensions.DependencyInjection.Abstractions)

## License

This project is provided as-is for demonstration and educational purposes.
