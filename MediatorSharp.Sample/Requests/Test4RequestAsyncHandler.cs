using MediatorSharp;

namespace MediatorSharp.Sample.Requests;

public class Test4RequestAsyncHandler : IAsyncRequestHandler<Test2Request>
{
    public Task<Result> HandleAsync(Test2Request request, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(Result.Success);
    }
}
