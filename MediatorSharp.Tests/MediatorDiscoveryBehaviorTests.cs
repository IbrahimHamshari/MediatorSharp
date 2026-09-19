using MediatorSharp.Tests.Fixtures;
using Microsoft.Extensions.DependencyInjection;

namespace MediatorSharp.Tests;

public class MediatorDiscoveryBehaviorTests
{
    [Fact]
    public void Discovered_NonGenericRequest_ExecutesPipeline()
    {
        using var provider = TestHost.BuildDiscovered();
        var mediator = provider.GetRequiredService<IMediator>();

        var result = mediator.Send(new PingRequest());

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task Discovered_NonGenericAsyncRequest_ExecutesPipeline()
    {
        using var provider = TestHost.BuildDiscovered();
        var mediator = provider.GetRequiredService<IMediator>();

        var result = await mediator.SendAsync(new AsyncPingRequest());

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public void Discovered_GenericRequest_ExecutesPipeline()
    {
        using var provider = TestHost.BuildDiscovered();
        var mediator = provider.GetRequiredService<IMediator>();

        var result = mediator.Send(new PongRequest("discovered"));

        Assert.True(result.IsSuccess);
        Assert.Equal("discovered", result.Value!.Value);
    }
}
