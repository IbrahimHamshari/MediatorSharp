using MediatorSharp;

namespace MediatorSharp.Sample.Requests;

public class Test2RequestHandler : IRequestHandler<Test2Request>
{
    public Result Handle(Test2Request request)
    {
        return Result.Success;
    }
}
