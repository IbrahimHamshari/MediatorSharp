namespace MediatorSharp.lib.Interfaces;

interface IResult
{
    bool IsSuccess { get; }
    IReadOnlyCollection<IError> Errors { get; }

}

interface IResult<T> : IResult where T : class
{
    T? Value { get; init; }
}