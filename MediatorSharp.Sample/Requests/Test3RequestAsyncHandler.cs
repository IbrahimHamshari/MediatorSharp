using MediatorSharp;
using MediatorSharp.Sample.Models;

namespace MediatorSharp.Sample.Requests;

public class Test3RequestAsyncHandler : IAsyncRequestHandler<TestRequest, Test>
{
    public Task<Result<Test>> HandleAsync(TestRequest request, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new Result<Test>(new Test { name = "test3" }));
    }
}
