namespace MediatorSharp.Tests.Fixtures;

public sealed class TestError : IError
{
    public TestError(string code, string message)
    {
        Code = code;
        Message = message;
    }

    public string Code { get; }

    public string Message { get; }
}
