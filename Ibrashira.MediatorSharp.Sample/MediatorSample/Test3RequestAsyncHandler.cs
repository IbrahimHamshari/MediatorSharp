using MediatorSharp.lib.Interfaces;
using MediatorSharp.lib.Models;
using MediatorSharp.Mediator;
using MediatorSharp.Models;

namespace Ibrashira.MediatorSharp.Sample.MediatorSample;

public class Test3RequestAsyncHandler : IAsyncRequestHandler<TestRequest, Test>
{
    public Task<Result<Test>> HandleAsync(TestRequest request, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new Result<Test>(new Test { name="test3"}));
    }
}
