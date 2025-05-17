using MediatorSharp.lib.Interfaces;
using MediatorSharp.lib.Models;
using MediatorSharp.Mediator;

namespace Ibrashira.MediatorSharp.Sample.MediatorSample;

public class Test4RequestAsyncHandler : IAsyncRequestHandler<Test2Request>
{
    public Task<Result> HandleAsync(Test2Request request, CancellationToken cancellationToken = default)
    {
        Console.WriteLine("xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx");
        return Task.FromResult(Result.Success);
    }
}
