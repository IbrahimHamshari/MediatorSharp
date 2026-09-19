using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace MediatorSharp.Tests.Fixtures;

public static class TestHost
{
    public static ServiceProvider BuildDiscovered()
    {
        var services = new ServiceCollection();
        services.AddSingleton(new CallLog());
        services.AddMediatorAndDiscoverRequestsWithPipelines(new[] { Assembly.GetExecutingAssembly() });
        return services.BuildServiceProvider();
    }

    public static ServiceProvider BuildManual(Action<IServiceCollection> configure)
    {
        var services = new ServiceCollection();
        services.AddSingleton<IMediator, Mediator>();
        configure(services);
        return services.BuildServiceProvider();
    }

    public static ServiceCollection Empty() => new();
}
