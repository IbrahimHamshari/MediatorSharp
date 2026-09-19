using MediatorSharp;
using MediatorSharp.Sample.Models;

namespace MediatorSharp.Sample.Requests;

public class PipelineTest3 : IAsyncPipelineBehavior<TestRequest, Test>
{
    public ILogger<PipelineTest3> Logger { get; }

    public PipelineTest3(ILogger<PipelineTest3> logger)
    {
        Logger = logger;
    }

    public async Task<Result<Test>> HandleAsync(TestRequest request, Func<TestRequest, Task<Result<Test>>> next, CancellationToken cancellationToken = default)
    {
        Logger.LogInformation("PipelineTest3: Before request handling");
        var response = await next(request);
        Logger.LogInformation("PipelineTest3: After request handling");
        return response;
    }
}
