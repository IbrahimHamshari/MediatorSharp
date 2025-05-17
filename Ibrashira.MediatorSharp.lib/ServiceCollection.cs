using MediatorSharp.lib.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace MediatorSharp.lib;

public static class ServiceCollection
{
    public static IServiceCollection AddMediatorAndDiscoverRequests(this IServiceCollection services, Assembly[] assemblies)
    {
        services.AddSingleton<IMediator, MediatorSharp.lib.Models.Mediator>();

        // Discover and register IApiRequest<> and IApiRequest<,> implementations
        var handlerTypes = assemblies
            .SelectMany(a => a.GetTypes())
            .Where(t => !t.IsAbstract && !t.IsInterface)
            .SelectMany(t => t.GetInterfaces()
                .Where(i => i.IsGenericType &&
                    (i.GetGenericTypeDefinition() == typeof(IRequestHandler<>) ||
                     i.GetGenericTypeDefinition() == typeof(IRequestHandler<,>) ||
                     i.GetGenericTypeDefinition() == typeof(IAsyncRequestHandler<>) ||
                     i.GetGenericTypeDefinition() == typeof(IAsyncRequestHandler<,>)))
                .Select(i => new { HandlerType = t, ServiceType = i }))
            .ToList();

        // Register each handler as its interface
        foreach (var handler in handlerTypes)
        {
            services.AddTransient(handler.ServiceType, handler.HandlerType);
        }

        return services;
    }

    public static IServiceCollection AddMediatorAndDiscoverRequestsWithPipelines(this IServiceCollection services, Assembly[] assemblies)
    {
        services.AddSingleton<IMediator, MediatorSharp.lib.Models.Mediator>();

        // Discover and register IApiRequest<> and IApiRequest<,> implementations
        var handlerTypes = assemblies
            .SelectMany(a => a.GetTypes())
            .Where(t => !t.IsAbstract && !t.IsInterface)
            .SelectMany(t => t.GetInterfaces()
                .Where(i => i.IsGenericType &&
                    (i.GetGenericTypeDefinition() == typeof(IRequestHandler<>) ||
                     i.GetGenericTypeDefinition() == typeof(IRequestHandler<,>) ||
                     i.GetGenericTypeDefinition() == typeof(IAsyncRequestHandler<>) ||
                     i.GetGenericTypeDefinition() == typeof(IAsyncRequestHandler<,>)))
                .Select(i => new { HandlerType = t, ServiceType = i }))
            .ToList();

        var pipelinesTypes = assemblies
            .SelectMany(a => a.GetTypes())
            .Where(t => !t.IsAbstract && !t.IsInterface)
            .SelectMany(t => t.GetInterfaces()
                .Where(i => i.IsGenericType &&
                    (i.GetGenericTypeDefinition() == typeof(IPipelineBehavior<>) ||
                     i.GetGenericTypeDefinition() == typeof(IPipelineBehavior<,>) ||
                     i.GetGenericTypeDefinition() == typeof(IAsyncPipelineBehavior<>) ||
                     i.GetGenericTypeDefinition() == typeof(IAsyncPipelineBehavior<,>)))
                .Select(i => new { HandlerType = t, ServiceType = i }))
            .ToList();

        // Register each handler as its interface
        foreach (var handler in handlerTypes)
        {
            services.AddTransient(handler.ServiceType, handler.HandlerType);
        }

        foreach(var pipeline in pipelinesTypes)
        {
            services.AddTransient(pipeline.ServiceType, pipeline.HandlerType);
        }

        return services;
    }
}
