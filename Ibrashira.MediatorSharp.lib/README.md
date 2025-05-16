# MediatorSharp

MediatorSharp is a lightweight, extensible mediator library for .NET 8, designed to decouple request handling and enable pipeline behaviors in your applications. This solution includes a reusable library (`MediatorSharp.lib`) and a sample ASP.NET Core Web API project (`MediatorSharp.Sample`) demonstrating its usage.

## Features

- **Request/Response Mediation**: Send requests and receive responses via strongly-typed handlers.
- **Pipeline Behaviors**: Add cross-cutting concerns (logging, validation, etc.) using pipeline behaviors.
- **Automatic Discovery**: Handlers and pipelines are automatically discovered and registered via reflection.
- **Integration with Dependency Injection**: Built on top of Microsoft.Extensions.DependencyInjection.

## Projects

- **MediatorSharp.lib**: Core mediator library.
- **MediatorSharp.Sample**: ASP.NET Core Web API sample demonstrating mediator usage.

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- Visual Studio 2022 or later

### Build and Run the Sample

1. Open the solution in Visual Studio.
2. Set `MediatorSharp.Sample` as the startup project.
3. Run the project (`F5` or `Ctrl+F5`).

The API will be available at `https://localhost:<port>/swagger` for interactive testing.

### Usage Overview

#### Registering Mediator and Pipelines

In `Program.cs`:

```csharp
builder.Services.AddMediatorAndDiscoverRequestsWithPipelines([Assembly.GetExecutingAssembly()]);
```

This registers all request handlers and pipeline behaviors found in the current assembly.

#### Defining Requests, Handlers, and Pipelines

- **Request**: Implement `IRequest` or `IRequest<TResponse>`.
- **Handler**: Implement `IRequestHandler<TRequest>` or `IRequestHandler<TRequest, TResponse>`.
- **Pipeline**: Implement `IPipelineBehavior<TRequest>` or `IPipelineBehavior<TRequest, TResponse>`.

#### Example Controller

```Csharp
[ApiController] [Route("[controller]")] public class SampleController : ControllerBase { private readonly IMediator _mediator;
public SampleController(IMediator mediator)
{
    _mediator = mediator;
}

[HttpGet("test1")]
public Result<Test> Get()
{
    var req = new TestRequest(1);
    return _mediator.Send(req);            
}

[HttpGet("test2")]
public Result GetTest2()
{
    var req = new Test2Request(1);
    return _mediator.Send(req);
}
}
```

## Extending

- Add new request/response types by implementing the appropriate interfaces.
- Add pipeline behaviors for logging, validation, etc., by implementing `IPipelineBehavior<>` or `IPipelineBehavior<,>`.

## Dependencies

- [Microsoft.Extensions.DependencyInjection.Abstractions](https://www.nuget.org/packages/Microsoft.Extensions.DependencyInjection.Abstractions)
- [Swashbuckle.AspNetCore](https://www.nuget.org/packages/Swashbuckle.AspNetCore) (for Swagger in the sample project)

## License

This project is provided as-is for demonstration and educational purposes.