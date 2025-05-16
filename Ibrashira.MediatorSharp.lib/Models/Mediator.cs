using MediatorSharp.lib.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace MediatorSharp.lib.Models;

public class Mediator : IMediator
{
    private readonly IServiceProvider _serviceProvider;

    public Mediator(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public Result Send(IRequest request)
    {
        var pipelines = GetAllRelevantPipelines(request).ToList();
        var concreteType = request.GetType();
        var handlerType = typeof(IRequestHandler<>).MakeGenericType(concreteType);
        var handler = _serviceProvider.GetRequiredService(handlerType);
        var handleMethod = handlerType.GetMethod("Handle");
        if (handleMethod == null)
            throw new InvalidOperationException($"Handler for {concreteType} does not have a Handle method.");

        // The final handler delegate
        Func<IRequest, Result> handlerDelegate = req =>
            (Result)handleMethod.Invoke(handler, new object[] { req })!;

        // Build the pipeline chain in reverse order
        foreach (var pipeline in pipelines.AsEnumerable().Reverse())
        {
            var next = handlerDelegate;
            var pipelineType = pipeline.GetType();
            var pipelineHandleMethod = pipelineType.GetMethod("Handle");
            if (pipelineHandleMethod == null)
                throw new InvalidOperationException($"Pipeline {pipelineType.Name} does not have a Handle method.");

            handlerDelegate = req =>
            {
                // Create the correct delegate type for the pipeline
                var delegateType = typeof(Func<,>).MakeGenericType(concreteType, typeof(Result));
                var pipelineDelegate = Delegate.CreateDelegate(delegateType, next.Target, next.Method);

                // Call pipeline.Handle(request, next)
                return (Result)pipelineHandleMethod.Invoke(pipeline, new object[] { req, pipelineDelegate })!;
            };
        }

        // Call the first pipeline (or the handler if no pipelines)
        return handlerDelegate(request);
    }

    public Result<T> Send<T>(IRequest<T> request) where T : class
    {
        var pipelines = GetAllRelevantGenericPipelines(request).ToList();
        var concreteType = request.GetType();
        var handlerType = typeof(IRequestHandler<,>).MakeGenericType(concreteType, typeof(T));
        var handler = _serviceProvider.GetRequiredService(handlerType);
        var handleMethod = handlerType.GetMethod("Handle");
        if (handleMethod == null)
            throw new InvalidOperationException($"Handler for {concreteType} and {typeof(T)} does not have a Handle method.");

        Func<IRequest<T>, Result<T>> handlerDelegate = req =>
            (Result<T>)handleMethod.Invoke(handler, new object[] { req })!;

        foreach (var pipeline in pipelines.AsEnumerable().Reverse())
        {
            var next = handlerDelegate;
            var pipelineType = pipeline.GetType();
            var pipelineHandleMethod = pipelineType.GetMethod("Handle");
            if (pipelineHandleMethod == null)
                throw new InvalidOperationException($"Pipeline {pipelineType.Name} does not have a Handle method.");

            handlerDelegate = req =>
            {
                var delegateType = typeof(Func<,>).MakeGenericType(concreteType, typeof(Result<T>));
                var pipelineDelegate = Delegate.CreateDelegate(delegateType, next.Target, next.Method);

                return (Result<T>)pipelineHandleMethod.Invoke(pipeline, new object[] { req, pipelineDelegate })!;
            };
        }

        return handlerDelegate(request);
    }

    private IEnumerable<object> GetAllRelevantPipelines(IRequest request)
    {
        var concreteType = request.GetType();
        var interfaces = concreteType.GetInterfaces();
        var pipelines = new List<object>();
        var concretePipelineType = typeof(IPipelineBehavior<>).MakeGenericType(concreteType);
        pipelines.AddRange(_serviceProvider.GetServices(concretePipelineType).OfType<object>());

        foreach (var iface in interfaces)
        {
            // Handle non-generic IRequest
            if (iface == typeof(IRequest))
            {
                var pipelineType = typeof(IPipelineBehavior<>).MakeGenericType(iface);
                pipelines.AddRange(_serviceProvider.GetServices(pipelineType).OfType<object>());
            }
        }

        return pipelines;
    }

    private IEnumerable<object> GetAllRelevantGenericPipelines<T>(IRequest<T> request) where T : class
    {
        var concreteType = request.GetType();
        var interfaces = concreteType.GetInterfaces();
        var pipelines = new List<object>();

        // Find the IRequest<T> interface implemented by the concrete type
        var iRequestInterface = interfaces
            .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequest<>));
        if (iRequestInterface == null)
            throw new InvalidOperationException($"Type {concreteType} does not implement IRequest<>.");

        var concreteResponseType = iRequestInterface.GetGenericArguments()[0];

        // This will be IPipelineBehavior<TestRequest, Test>
        var concretePipelineType = typeof(IPipelineBehavior<,>).MakeGenericType(concreteType, concreteResponseType);
        pipelines.AddRange(_serviceProvider.GetServices(concretePipelineType).OfType<object>());

        foreach (var iface in interfaces)
        {
            // Handle generic IRequest<T>
            if (iface.IsGenericType && iface.GetGenericTypeDefinition() == typeof(IRequest<>))
            {
                var responseType = iface.GetGenericArguments()[0];
                var pipelineType = typeof(IPipelineBehavior<,>).MakeGenericType(iface, responseType);
                pipelines.AddRange(_serviceProvider.GetServices(pipelineType).OfType<object>());
            }
        }

        return pipelines;
    }
}