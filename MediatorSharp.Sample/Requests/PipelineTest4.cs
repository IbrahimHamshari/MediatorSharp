using MediatorSharp;

namespace MediatorSharp.Sample.Requests;

public class PipelineTest4 : IAsyncPipelineBehavior<Test2Request>
{
    public ILogger<PipelineTest4> Logger { get; }

    public PipelineTest4(ILogger<PipelineTest4> logger)
    {
        Logger = logger;
    }

    public async Task<Result> HandleAsync(Test2Request request, Func<Test2Request, Task<Result>> next, CancellationToken cancellationToken = default)
    {
        Logger.LogInformation("PipelineTest4: Before request handling");
        var response = await next(request);
        Logger.LogInformation("PipelineTest4: After request handling");
        return response;
    }
}
