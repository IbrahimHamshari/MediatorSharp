using MediatorSharp;
using MediatorSharp.Sample.Models;

namespace MediatorSharp.Sample.Requests;

public class PipelineTest2 : IPipelineBehavior<TestRequest, Test>
{
    public ILogger<PipelineTest2> Logger { get; }

    public PipelineTest2(ILogger<PipelineTest2> logger)
    {
        Logger = logger;
    }


    public Result<Test> Handle(TestRequest request, Func<TestRequest, Result<Test>> next)
    {
        Logger.LogInformation("PipelineTest2: Before request handling");
        var result = next(request);
        Logger.LogInformation("PipelineTest2: After request handling");
        return result;
    }
}
