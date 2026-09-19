using System.Reflection;
using MediatorSharp.Tests.Fixtures;
using Microsoft.Extensions.DependencyInjection;

namespace MediatorSharp.Tests;

public class ServiceCollectionExtensionsTests
{
    [Fact]
    public void AddMediatorAndDiscoverRequests_RegistersMediator()
    {
        var services = new ServiceCollection();

        services.AddMediatorAndDiscoverRequests(new[] { Assembly.GetExecutingAssembly() });

        using var provider = services.BuildServiceProvider();
        Assert.NotNull(provider.GetService<IMediator>());
    }

    [Fact]
    public void AddMediatorAndDiscoverRequests_RegistersAllHandlerKinds()
    {
        var services = new ServiceCollection();

        services.AddMediatorAndDiscoverRequests(new[] { Assembly.GetExecutingAssembly() });

        using var provider = services.BuildServiceProvider();
        Assert.NotNull(provider.GetService<IRequestHandler<PingRequest>>());
        Assert.NotNull(provider.GetService<IRequestHandler<PongRequest, PongResponse>>());
        Assert.NotNull(provider.GetService<IAsyncRequestHandler<AsyncPingRequest>>());
        Assert.NotNull(provider.GetService<IAsyncRequestHandler<AsyncPongRequest, PongResponse>>());
    }

    [Fact]
    public void AddMediatorAndDiscoverRequests_DoesNotRegisterPipelines()
    {
        var services = new ServiceCollection();

        services.AddMediatorAndDiscoverRequests(new[] { Assembly.GetExecutingAssembly() });

        using var provider = services.BuildServiceProvider();
        Assert.Empty(provider.GetServices<IPipelineBehavior<PingRequest>>());
        Assert.Empty(provider.GetServices<IAsyncPipelineBehavior<AsyncPingRequest>>());
    }

    [Fact]
    public void AddMediatorAndDiscoverRequestsWithPipelines_RegistersAllPipelineKinds()
    {
        var services = new ServiceCollection();
        services.AddSingleton(new CallLog());

        services.AddMediatorAndDiscoverRequestsWithPipelines(new[] { Assembly.GetExecutingAssembly() });

        using var provider = services.BuildServiceProvider();
        Assert.NotEmpty(provider.GetServices<IPipelineBehavior<PingRequest>>());
        Assert.NotEmpty(provider.GetServices<IPipelineBehavior<PongRequest, PongResponse>>());
        Assert.NotEmpty(provider.GetServices<IAsyncPipelineBehavior<AsyncPingRequest>>());
        Assert.NotEmpty(provider.GetServices<IAsyncPipelineBehavior<AsyncPongRequest, PongResponse>>());
    }

    [Fact]
    public void AddMediatorAndDiscoverRequestsWithPipelines_RegistersHandlersAndMediator()
    {
        var services = new ServiceCollection();

        services.AddMediatorAndDiscoverRequestsWithPipelines(new[] { Assembly.GetExecutingAssembly() });

        using var provider = services.BuildServiceProvider();
        Assert.NotNull(provider.GetService<IMediator>());
        Assert.NotNull(provider.GetService<IRequestHandler<PingRequest>>());
    }

    [Fact]
    public void Discovery_IgnoresAbstractAndInterfaceTypes()
    {
        var services = new ServiceCollection();

        services.AddMediatorAndDiscoverRequestsWithPipelines(new[] { Assembly.GetExecutingAssembly() });

        var descriptors = services.Where(d =>
            d.ServiceType.IsGenericType &&
            (d.ServiceType.GetGenericTypeDefinition() == typeof(IRequestHandler<>) ||
             d.ServiceType.GetGenericTypeDefinition() == typeof(IPipelineBehavior<>))).ToList();

        Assert.All(descriptors, d => Assert.False(d.ImplementationType!.IsAbstract));
        Assert.All(descriptors, d => Assert.True(d.ImplementationType!.IsClass));
    }

    [Fact]
    public void AddMediatorAndDiscoverRequests_ReturnsSameCollection()
    {
        var services = new ServiceCollection();

        var returned = services.AddMediatorAndDiscoverRequests(new[] { Assembly.GetExecutingAssembly() });

        Assert.Same(services, returned);
    }
}
