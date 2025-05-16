using MediatorSharp.lib.Interfaces;
using MediatorSharp.lib.Models;

namespace MediatorSharp.Mediator;

public class Test2RequestHandler : IRequestHandler<Test2Request>
{
    public Result Handle(Test2Request request)
    {
        return Result.Success;
    }
}
