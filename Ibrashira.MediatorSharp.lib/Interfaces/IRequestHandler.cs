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
