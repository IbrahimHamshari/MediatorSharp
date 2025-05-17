using MediatorSharp.lib.Models;

namespace MediatorSharp.lib.Interfaces;

public interface IRequestHandler<T> where T : IRequest
{
    public Result Handle(T request);
}

public interface IRequestHandler<T, S> where T : IRequest<S> where S : class
{
    public Result<S> Handle(T request);
}

public interface IAsyncRequestHandler<TRequest> where TRequest : IRequest
{
    Task<Result> HandleAsync(TRequest request, CancellationToken cancellationToken = default);
}

public interface IAsyncRequestHandler<TRequest, TResponse> where TRequest : IRequest<TResponse> where TResponse : class
{
    Task<Result<TResponse>> HandleAsync(TRequest request, CancellationToken cancellationToken = default);
}