namespace MediatorSharp;

public interface IResult
{
    bool IsSuccess { get; }
    IReadOnlyCollection<IError> Errors { get; }
}

public interface IResult<T> : IResult where T : class
{
    T? Value { get; }
}
