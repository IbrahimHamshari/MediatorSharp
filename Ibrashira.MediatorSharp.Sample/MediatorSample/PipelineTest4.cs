using MediatorSharp.lib.Interfaces;
using MediatorSharp.lib.Models;
using MediatorSharp.Mediator;

namespace Ibrashira.MediatorSharp.Sample.MediatorSample;

public class PipelineTest4 : IAsyncPipelineBehavior<Test2Request>
{
    public ILogger<PipelineTest3> Logger { get; }

    public PipelineTest4(ILogger<PipelineTest3> logger)
    {
        Logger = logger;
    }

    public async Task<Result> HandleAsync(Test2Request request, Func<Test2Request, Task<Result>> next, CancellationToken cancellationToken = default)
    {
        Logger.LogInformation("PipelineTest4: Before request handling");
        var response = await next(request);
        Logger.LogInformation("PipelineTest4: Before request handling");
        return response;
    }
}