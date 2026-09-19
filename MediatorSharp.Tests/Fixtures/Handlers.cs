namespace MediatorSharp.Tests.Fixtures;

public sealed class PingHandler : IRequestHandler<PingRequest>
{
    public Result Handle(PingRequest request) => Result.Success;
}

public sealed class PongHandler : IRequestHandler<PongRequest, PongResponse>
{
    public Result<PongResponse> Handle(PongRequest request) =>
        new PongResponse { Value = request.Value };
}

public sealed class AsyncPingHandler : IAsyncRequestHandler<AsyncPingRequest>
{
    public Task<Result> HandleAsync(AsyncPingRequest request, CancellationToken cancellationToken = default) =>
        Task.FromResult(Result.Success);
}

public sealed class AsyncPongHandler : IAsyncRequestHandler<AsyncPongRequest, PongResponse>
{
    public Task<Result<PongResponse>> HandleAsync(AsyncPongRequest request, CancellationToken cancellationToken = default) =>
        Task.FromResult(new Result<PongResponse>(new PongResponse { Value = request.Value }));
}

public sealed class ExplicitHandler : IRequestHandler<ExplicitRequest>
{
    Result IRequestHandler<ExplicitRequest>.Handle(ExplicitRequest request) => Result.Success;
}
