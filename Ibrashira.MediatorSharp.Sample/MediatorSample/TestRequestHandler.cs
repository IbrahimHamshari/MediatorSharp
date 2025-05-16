using MediatorSharp.lib.Interfaces;
using MediatorSharp.lib.Models;
using MediatorSharp.Models;

namespace MediatorSharp.Mediator;

public class TestRequestHandler : IRequestHandler<TestRequest, Test>
{
    public Result<Test> Handle(TestRequest request)
    {
        var test = new Test { name = "Test" };
        return test;
    }
}
