using MediatorSharp.Tests.Fixtures;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;

namespace MediatorSharp.Tests;

public class MediatorInteractionTests
{
    [Fact]
    public void Send_DelegatesToResolvedHandler()
    {
        var handler = Substitute.For<IRequestHandler<PingRequest>>();
        handler.Handle(Arg.Any<PingRequest>()).Returns(Result.Success);
        using var provider = TestHost.BuildManual(s => s.AddSingleton(handler));
        var mediator = provider.GetRequiredService<IMediator>();

        mediator.Send(new PingRequest());

        handler.Received(1).Handle(Arg.Any<PingRequest>());
    }

    [Fact]
    public async Task SendAsync_DelegatesToResolvedHandler()
    {
        var handler = Substitute.For<IAsyncRequestHandler<AsyncPingRequest>>();
        handler.HandleAsync(Arg.Any<AsyncPingRequest>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result.Success));
        using var provider = TestHost.BuildManual(s => s.AddSingleton(handler));
        var mediator = provider.GetRequiredService<IMediator>();

        await mediator.SendAsync(new AsyncPingRequest());

        await handler.Received(1).HandleAsync(Arg.Any<AsyncPingRequest>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public void Send_HandlerFailure_PropagatesErrors()
    {
        var error = new TestError("FAIL", "handler failed");
        var handler = Substitute.For<IRequestHandler<PingRequest>>();
        handler.Handle(Arg.Any<PingRequest>()).Returns(Result.FromError(error));
        using var provider = TestHost.BuildManual(s => s.AddSingleton(handler));
        var mediator = provider.GetRequiredService<IMediator>();

        var result = mediator.Send(new PingRequest());

        Assert.False(result.IsSuccess);
        Assert.Same(error, result.Errors.Single());
    }
}
