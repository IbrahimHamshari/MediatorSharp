namespace MediatorSharp;

public interface IError
{
    string Code { get; }

    string Message { get; }
}
