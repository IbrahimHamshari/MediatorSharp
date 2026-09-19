namespace MediatorSharp;

public class Result : IResult
{
    public IReadOnlyCollection<IError> Errors { get; init; } = Array.Empty<IError>();

    public bool IsSuccess => Errors.Count == 0;

    public static Result FromError(IError error) => new Result(error);

    public static Result Success => new Result();

    public Result(IError error)
    {
        Errors = new[] { error };
    }

    private Result() {}
}

public class Result<T> : IResult<T> where T : class
{
    public bool IsSuccess => Errors.Count == 0;

    public IReadOnlyCollection<IError> Errors { get; init; } = Array.Empty<IError>();

    public T? Value { get; init; }

    public Result(T value)
    {
        Value = value;
    }

    public Result(IError error)
    {
        Errors = new[] { error };
    }

    public Result(ICollection<IError> errors)
    {
        Errors = errors.ToArray();
    }

    public static Result<T> FromError(IError error) => new Result<T>(error);

    public static implicit operator Result<T>(T result)
    {
        return new Result<T>(result);
    }
}
