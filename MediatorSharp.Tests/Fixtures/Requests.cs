namespace MediatorSharp.Tests.Fixtures;

public record PingRequest : IRequest;

public record PongRequest(string Value) : IRequest<PongResponse>;

public record AsyncPingRequest : IRequest;

public record AsyncPongRequest(string Value) : IRequest<PongResponse>;

public record ExplicitRequest : IRequest;

public record NoHandlerRequest : IRequest;

public record NoHandlerPongRequest : IRequest<PongResponse>;
