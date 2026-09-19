namespace MediatorSharp.Tests.Fixtures;

public sealed class PingPipeline : IPipelineBehavior<PingRequest>
{
    private readonly CallLog _log;

    public PingPipeline(CallLog log) => _log = log;

    public Result Handle(PingRequest request, Func<PingRequest, Result> next)
    {
        _log.Add("PingPipeline:before");
        var result = next(request);
        _log.Add("PingPipeline:after");
        return result;
    }
}

public sealed class PongPipeline : IPipelineBehavior<PongRequest, PongResponse>
{
    private readonly CallLog _log;

    public PongPipeline(CallLog log) => _log = log;

    public Result<PongResponse> Handle(PongRequest request, Func<PongRequest, Result<PongResponse>> next)
    {
        _log.Add("PongPipeline:before");
        var result = next(request);
        _log.Add("PongPipeline:after");
        return result;
    }
}

public sealed class AsyncPingPipeline : IAsyncPipelineBehavior<AsyncPingRequest>
{
    private readonly CallLog _log;

    public AsyncPingPipeline(CallLog log) => _log = log;

    public async Task<Result> HandleAsync(AsyncPingRequest request, Func<AsyncPingRequest, Task<Result>> next, CancellationToken cancellationToken = default)
    {
        _log.Add("AsyncPingPipeline:before");
        var result = await next(request);
        _log.Add("AsyncPingPipeline:after");
        return result;
    }
}

public sealed class AsyncPongPipeline : IAsyncPipelineBehavior<AsyncPongRequest, PongResponse>
{
    private readonly CallLog _log;

    public AsyncPongPipeline(CallLog log) => _log = log;

    public async Task<Result<PongResponse>> HandleAsync(AsyncPongRequest request, Func<AsyncPongRequest, Task<Result<PongResponse>>> next, CancellationToken cancellationToken = default)
    {
        _log.Add("AsyncPongPipeline:before");
        var result = await next(request);
        _log.Add("AsyncPongPipeline:after");
        return result;
    }
}

public sealed class FirstOrderPipeline : IPipelineBehavior<PingRequest>
{
    private readonly CallLog _log;

    public FirstOrderPipeline(CallLog log) => _log = log;

    public Result Handle(PingRequest request, Func<PingRequest, Result> next)
    {
        _log.Add("First:before");
        var result = next(request);
        _log.Add("First:after");
        return result;
    }
}

public sealed class SecondOrderPipeline : IPipelineBehavior<PingRequest>
{
    private readonly CallLog _log;

    public SecondOrderPipeline(CallLog log) => _log = log;

    public Result Handle(PingRequest request, Func<PingRequest, Result> next)
    {
        _log.Add("Second:before");
        var result = next(request);
        _log.Add("Second:after");
        return result;
    }
}
