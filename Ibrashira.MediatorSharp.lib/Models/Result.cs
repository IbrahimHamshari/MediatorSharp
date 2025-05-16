using MediatorSharp.lib.Interfaces;
using System.Runtime.CompilerServices;

namespace MediatorSharp.lib.Models;

public class Result : IResult
{
    public Result(IError error)
    {
        Errors.Append(error);
    }

    public Result( )
    {
    }

    public IReadOnlyCollection<IError> Errors { get; init; } = Array.Empty<IError>();

    public bool IsSuccess => Errors.Count == 0;

    public static Result FromError(IError error) => new Result(error);
    
    public static Result Success => new Result();
}

public class Result<T> : IResult<T> where T : class
{
    public Result(T value)
    {
        Value = value;
    }
    public Result(IError error)
    {
        Errors.Append(error);
    }

    public Result(ICollection<IError> errors)
    {
        Errors.Concat(errors);
    }

    public IReadOnlyCollection<IError> Errors { get; init; } = Array.Empty<IError>();

    public bool IsSuccess => Errors.Count == 0;

    public T? Value { get; init; }

    public static Result FromError(IError error) => new Result(error);

    public static implicit operator Result<T>(T result)
    {
        return new Result<T>(result);
    }

}
