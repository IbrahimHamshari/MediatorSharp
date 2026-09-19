using MediatorSharp;

namespace MediatorSharp.Sample.Requests;

public class PipelineTest : IPipelineBehavior<Test2Request>
{
    public ILogger<PipelineTest> Logger { get; }

    public PipelineTest(ILogger<PipelineTest> logger)
    {
        Logger = logger;
    }

    public Result Handle(Test2Request request, Func<Test2Request, Result> next)
    {
        Logger.LogInformation("PipelineTest: Before request handling");
        var result = next(request);
        Logger.LogInformation("PipelineTest: After request handling");
        return result;
    }
}
