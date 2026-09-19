using MediatorSharp;
using MediatorSharp.Sample.Models;

namespace MediatorSharp.Sample.Requests;

public class TestRequestHandler : IRequestHandler<TestRequest, Test>
{
    public Result<Test> Handle(TestRequest request)
    {
        var test = new Test { name = "Test" };
        return test;
    }
}
