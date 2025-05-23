﻿using MediatorSharp.lib.Interfaces;
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

        // The final handler delegate
        Func<IRequest<T>, Result<T>> handlerDelegate = req =>
            (Result<T>)handleMethod.Invoke(handler, new object[] { req })!;

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
                var delegateType = typeof(Func<,>).MakeGenericType(concreteType, typeof(Result<T>));
                var pipelineDelegate = Delegate.CreateDelegate(delegateType, next.Target, next.Method);

                // Call pipeline.Handle(request, next)
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

        // This will be IPipelineBehavior<TestRequest>
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

    public async Task<Result> SendAsync(IRequest request, CancellationToken cancellationToken = default)
    {
        var pipelines = GetAllRelevantAsyncPipelines(request).ToList();
        var concreteType = request.GetType();
        var handlerType = typeof(IAsyncRequestHandler<>).MakeGenericType(concreteType);
        var handler = _serviceProvider.GetRequiredService(handlerType);
        var handleMethod = handlerType.GetMethod("HandleAsync");

        if (handleMethod == null)
            throw new InvalidOperationException($"Handler for {concreteType} does not have a HandleAsync method.");

        // The final handler delegate
        Func<IRequest, CancellationToken, Task<Result>> handlerDelegate = (req, ct) =>
            (Task<Result>)handleMethod.Invoke(handler, new object[] { req, ct })!;

        // Build the pipeline chain in reverse order
        foreach (var pipeline in pipelines.AsEnumerable().Reverse())
        {
            var next = handlerDelegate;
            var pipelineType = pipeline.GetType();
            var pipelineHandleMethod = pipelineType.GetMethod("HandleAsync");
            if (pipelineHandleMethod == null)
                throw new InvalidOperationException($"Pipeline {pipelineType.Name} does not have a HandleAsync method.");

            handlerDelegate = (req, ct) =>
            {
                Func<IRequest, Task<Result>> nextWithoutCt = r => next(r, ct);

                // Create the correct delegate type for the pipeline
                var delegateType = typeof(Func<,>).MakeGenericType(concreteType, typeof(Task<Result>));
                var pipelineDelegate = Delegate.CreateDelegate(delegateType, nextWithoutCt.Target, nextWithoutCt.Method);

                // Call pipeline.Handle(request, next, ct)
                return (Task<Result>)pipelineHandleMethod.Invoke(pipeline, new object[] { req, pipelineDelegate, ct })!;
            };
        }

        return await handlerDelegate(request, cancellationToken);
    }

    public async Task<Result<T>> SendAsync<T>(IRequest<T> request, CancellationToken cancellationToken = default) where T : class
    {
        var pipelines = GetAllRelevantAsyncGenericPipelines(request).ToList();
        var concreteType = request.GetType();
        var handlerType = typeof(IAsyncRequestHandler<,>).MakeGenericType(concreteType, typeof(T));
        var handler = _serviceProvider.GetRequiredService(handlerType);
        var handleMethod = handlerType.GetMethod("HandleAsync");
        if (handleMethod == null)
            throw new InvalidOperationException($"Handler for {concreteType} and {typeof(T)} does not have a HandleAsync method.");

        // The final handler delegate
        Func<IRequest<T>, CancellationToken, Task<Result<T>>> handlerDelegate = (req, ct) =>
            (Task<Result<T>>)handleMethod.Invoke(handler, new object[] { req, ct })!;

        // Build the pipeline chain in reverse order
        foreach (var pipeline in pipelines.AsEnumerable().Reverse())
        {
            var next = handlerDelegate;
            var pipelineType = pipeline.GetType();
            var pipelineHandleMethod = pipelineType.GetMethod("HandleAsync");
            if (pipelineHandleMethod == null)
                throw new InvalidOperationException($"Pipeline {pipelineType.Name} does not have a HandleAsync method.");

            handlerDelegate = (req, ct) =>
            {
                Func<IRequest<T>, Task<Result<T>>> nextWithoutCt = r => next(r, ct);

                // Create the correct delegate type for the pipeline
                var delegateType = typeof(Func<,>).MakeGenericType(concreteType, typeof(Task<Result<T>>));
                var pipelineDelegate = Delegate.CreateDelegate(delegateType, nextWithoutCt.Target, nextWithoutCt.Method);

                // Call pipeline.Handle(request, next, ct)
                return (Task<Result<T>>)pipelineHandleMethod.Invoke(pipeline, new object[] { req, pipelineDelegate, ct })!;

            };
        }

        return await handlerDelegate(request, cancellationToken);
    }

    private IEnumerable<object> GetAllRelevantAsyncPipelines(IRequest request)
    {
        var concreteType = request.GetType();
        var interfaces = concreteType.GetInterfaces();
        var pipelines = new List<object>();

        // This will be IPipelineBehavior<TestRequest>
        var concretePipelineType = typeof(IAsyncPipelineBehavior<>).MakeGenericType(concreteType);
        pipelines.AddRange(_serviceProvider.GetServices(concretePipelineType).OfType<object>());

        foreach (var iface in interfaces)
        {
            // Handle non-generic IRequest
            if (iface == typeof(IRequest))
            {
                var pipelineType = typeof(IAsyncPipelineBehavior<>).MakeGenericType(iface);
                pipelines.AddRange(_serviceProvider.GetServices(pipelineType).OfType<object>());
            }
        }

        return pipelines;
    }

    private IEnumerable<object> GetAllRelevantAsyncGenericPipelines<T>(IRequest<T> request) where T : class
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
        var concretePipelineType = typeof(IAsyncPipelineBehavior<,>).MakeGenericType(concreteType, concreteResponseType);
        pipelines.AddRange(_serviceProvider.GetServices(concretePipelineType).OfType<object>());

        foreach (var iface in interfaces)
        {
            // Handle generic IRequest<T>
            if (iface.IsGenericType && iface.GetGenericTypeDefinition() == typeof(IRequest<>))
            {
                var responseType = iface.GetGenericArguments()[0];
                var pipelineType = typeof(IAsyncPipelineBehavior<,>).MakeGenericType(iface, responseType);
                pipelines.AddRange(_serviceProvider.GetServices(pipelineType).OfType<object>());
            }
        }

        return pipelines;
    }
}