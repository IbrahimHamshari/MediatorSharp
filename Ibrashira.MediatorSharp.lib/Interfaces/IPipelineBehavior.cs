using MediatorSharp.lib.Models;

namespace MediatorSharp.lib.Interfaces;

public interface IPipelineBehavior<T> where T : IRequest
{
    public Result Handle(T request, Func<T, Result> next);
}

public interface IPipelineBehavior<T, S> where T : IRequest<S> where S : class
{
    public Result<S> Handle(T request, Func<T, Result<S>> next);
}

public interface IAsyncPipelineBehavior<T> where T : IRequest
{
    Task<Result> HandleAsync(T request, Func<T, Task<Result>> next, CancellationToken cancellationToken = default);
}

public interface IAsyncPipelineBehavior<T, S> where T : IRequest<S> where S : class
{
    Task<Result<S>> HandleAsync(T request, Func<T, Task<Result<S>>> next, CancellationToken cancellationToken = default);
}