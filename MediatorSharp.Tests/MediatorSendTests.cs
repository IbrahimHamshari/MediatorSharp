using MediatorSharp.Tests.Fixtures;
using Microsoft.Extensions.DependencyInjection;

namespace MediatorSharp.Tests;

public class MediatorSendTests
{
    [Fact]
    public void Send_NonGenericRequest_ReturnsSuccess()
    {
        using var provider = TestHost.BuildManual(s =>
            s.AddTransient<IRequestHandler<PingRequest>, PingHandler>());
        var mediator = provider.GetRequiredService<IMediator>();

        var result = mediator.Send(new PingRequest());

        Assert.True(result.IsSuccess);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public void Send_GenericRequest_ReturnsHandlerValue()
    {
        using var provider = TestHost.BuildManual(s =>
            s.AddTransient<IRequestHandler<PongRequest, PongResponse>, PongHandler>());
        var mediator = provider.GetRequiredService<IMediator>();

        var result = mediator.Send(new PongRequest("hello"));

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal("hello", result.Value!.Value);
    }

    [Fact]
    public void Send_WithoutHandler_Throws()
    {
        using var provider = TestHost.BuildManual(_ => { });
        var mediator = provider.GetRequiredService<IMediator>();

        Assert.ThrowsAny<Exception>(() => mediator.Send(new NoHandlerRequest()));
    }

    [Fact]
    public void Send_WithoutGenericHandler_Throws()
    {
        using var provider = TestHost.BuildManual(_ => { });
        var mediator = provider.GetRequiredService<IMediator>();

        Assert.ThrowsAny<Exception>(() => mediator.Send(new NoHandlerPongRequest()));
    }

    [Fact]
    public void Send_ExplicitInterfaceHandler_ReturnsSuccess()
    {
        using var provider = TestHost.BuildManual(s =>
            s.AddTransient<IRequestHandler<ExplicitRequest>, ExplicitHandler>());
        var mediator = provider.GetRequiredService<IMediator>();

        var result = mediator.Send(new ExplicitRequest());

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public void Send_WithPipeline_InvokesPipelineAroundHandler()
    {
        var log = new CallLog();
        using var provider = TestHost.BuildManual(s =>
        {
            s.AddSingleton(log);
            s.AddTransient<IRequestHandler<PingRequest>, PingHandler>();
            s.AddTransient<IPipelineBehavior<PingRequest>, PingPipeline>();
        });
        var mediator = provider.GetRequiredService<IMediator>();

        var result = mediator.Send(new PingRequest());

        Assert.True(result.IsSuccess);
        Assert.Equal(new[] { "PingPipeline:before", "PingPipeline:after" }, log.Entries);
    }

    [Fact]
    public void Send_WithGenericPipeline_InvokesPipelineAroundHandler()
    {
        var log = new CallLog();
        using var provider = TestHost.BuildManual(s =>
        {
            s.AddSingleton(log);
            s.AddTransient<IRequestHandler<PongRequest, PongResponse>, PongHandler>();
            s.AddTransient<IPipelineBehavior<PongRequest, PongResponse>, PongPipeline>();
        });
        var mediator = provider.GetRequiredService<IMediator>();

        var result = mediator.Send(new PongRequest("pong"));

        Assert.True(result.IsSuccess);
        Assert.Equal("pong", result.Value!.Value);
        Assert.Equal(new[] { "PongPipeline:before", "PongPipeline:after" }, log.Entries);
    }

    [Fact]
    public void Send_WithMultiplePipelines_FirstRegisteredIsOutermost()
    {
        var log = new CallLog();
        using var provider = TestHost.BuildManual(s =>
        {
            s.AddSingleton(log);
            s.AddTransient<IRequestHandler<PingRequest>, PingHandler>();
            s.AddTransient<IPipelineBehavior<PingRequest>, FirstOrderPipeline>();
            s.AddTransient<IPipelineBehavior<PingRequest>, SecondOrderPipeline>();
        });
        var mediator = provider.GetRequiredService<IMediator>();

        mediator.Send(new PingRequest());

        Assert.Equal(
            new[] { "First:before", "Second:before", "Second:after", "First:after" },
            log.Entries);
    }
}
