using MediatorSharp.Tests.Fixtures;
using Microsoft.Extensions.DependencyInjection;

namespace MediatorSharp.Tests;

public class MediatorSendAsyncTests
{
    [Fact]
    public async Task SendAsync_NonGenericRequest_ReturnsSuccess()
    {
        using var provider = TestHost.BuildManual(s =>
            s.AddTransient<IAsyncRequestHandler<AsyncPingRequest>, AsyncPingHandler>());
        var mediator = provider.GetRequiredService<IMediator>();

        var result = await mediator.SendAsync(new AsyncPingRequest());

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task SendAsync_GenericRequest_ReturnsHandlerValue()
    {
        using var provider = TestHost.BuildManual(s =>
            s.AddTransient<IAsyncRequestHandler<AsyncPongRequest, PongResponse>, AsyncPongHandler>());
        var mediator = provider.GetRequiredService<IMediator>();

        var result = await mediator.SendAsync(new AsyncPongRequest("async-hello"));

        Assert.True(result.IsSuccess);
        Assert.Equal("async-hello", result.Value!.Value);
    }

    [Fact]
    public async Task SendAsync_WithPipeline_InvokesPipelineAroundHandler()
    {
        var log = new CallLog();
        using var provider = TestHost.BuildManual(s =>
        {
            s.AddSingleton(log);
            s.AddTransient<IAsyncRequestHandler<AsyncPingRequest>, AsyncPingHandler>();
            s.AddTransient<IAsyncPipelineBehavior<AsyncPingRequest>, AsyncPingPipeline>();
        });
        var mediator = provider.GetRequiredService<IMediator>();

        var result = await mediator.SendAsync(new AsyncPingRequest());

        Assert.True(result.IsSuccess);
        Assert.Equal(new[] { "AsyncPingPipeline:before", "AsyncPingPipeline:after" }, log.Entries);
    }

    [Fact]
    public async Task SendAsync_WithGenericPipeline_InvokesPipelineAroundHandler()
    {
        var log = new CallLog();
        using var provider = TestHost.BuildManual(s =>
        {
            s.AddSingleton(log);
            s.AddTransient<IAsyncRequestHandler<AsyncPongRequest, PongResponse>, AsyncPongHandler>();
            s.AddTransient<IAsyncPipelineBehavior<AsyncPongRequest, PongResponse>, AsyncPongPipeline>();
        });
        var mediator = provider.GetRequiredService<IMediator>();

        var result = await mediator.SendAsync(new AsyncPongRequest("async-pong"));

        Assert.True(result.IsSuccess);
        Assert.Equal("async-pong", result.Value!.Value);
        Assert.Equal(new[] { "AsyncPongPipeline:before", "AsyncPongPipeline:after" }, log.Entries);
    }

    [Fact]
    public async Task SendAsync_WithoutHandler_Throws()
    {
        using var provider = TestHost.BuildManual(_ => { });
        var mediator = provider.GetRequiredService<IMediator>();

        await Assert.ThrowsAnyAsync<Exception>(() => mediator.SendAsync(new NoHandlerRequest()));
    }

    [Fact]
    public async Task SendAsync_WithoutGenericHandler_Throws()
    {
        using var provider = TestHost.BuildManual(_ => { });
        var mediator = provider.GetRequiredService<IMediator>();

        await Assert.ThrowsAnyAsync<Exception>(() => mediator.SendAsync(new NoHandlerPongRequest()));
    }

    [Fact]
    public async Task SendAsync_PassesCancellationTokenThroughPipeline()
    {
        var log = new CallLog();
        using var provider = TestHost.BuildManual(s =>
        {
            s.AddSingleton(log);
            s.AddTransient<IAsyncRequestHandler<AsyncPingRequest>, AsyncPingHandler>();
            s.AddTransient<IAsyncPipelineBehavior<AsyncPingRequest>, AsyncPingPipeline>();
        });
        var mediator = provider.GetRequiredService<IMediator>();

        using var cts = new CancellationTokenSource();
        var result = await mediator.SendAsync(new AsyncPingRequest(), cts.Token);

        Assert.True(result.IsSuccess);
        Assert.Equal(new[] { "AsyncPingPipeline:before", "AsyncPingPipeline:after" }, log.Entries);
    }
}
