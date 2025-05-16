using MediatorSharp.lib.Models;

namespace MediatorSharp.lib.Interfaces;

public interface IPipelineBehavior<T> where T: IRequest
{
    public Result Handle(T request, Func<T, Result> next);
}

public interface IPipelineBehavior<T, S> where T: IRequest<S> where S: class
{
    public Result<S> Handle(T request, Func<T, Result<S>> next);
}

