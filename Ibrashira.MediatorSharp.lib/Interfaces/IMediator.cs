using MediatorSharp.lib.Models;

namespace MediatorSharp.lib.Interfaces;

interface IMediator
{
    Result Send(IRequest request);

    Result<T> Send<T>(IRequest<T> request) where T : class;

    Task<Result> SendAsync(IRequest request, CancellationToken cancellationToken = default);

    Task<Result<T>> SendAsync<T>(IRequest<T> request, CancellationToken cancellationToken = default) where T : class;
}