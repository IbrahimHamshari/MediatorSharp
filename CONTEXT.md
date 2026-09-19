# MediatorSharp

MediatorSharp is an in-process mediator for .NET. It decouples a caller that wants
something done from the code that does it, and lets cross-cutting behavior wrap that
work without the caller knowing.

## Language

**Mediator**:
The component a caller sends a request to. It resolves the request's handler, wraps it
in any matching pipeline behaviors, and returns the handler's result.
_Avoid_: Dispatcher, bus, broker

**Request**:
A message describing an intent to be handled. A request declares its response type, or
declares none. It carries data and behavior, not the work itself.
_Avoid_: Command, query, message

**Response**:
The value a request declares it will produce. A request with no response is handled for
its side effects and reports success or failure only.
_Avoid_: Result value, payload, output

**Handler**:
The single piece of code that fulfills one request. Exactly one handler exists per
request type. A handler may be synchronous or asynchronous.
_Avoid_: Processor, executor, service

**Pipeline Behavior**:
Cross-cutting logic that wraps a handler — logging, validation, timing. Behaviors nest,
each calling the next. A behavior is matched to a request the same way a handler is.
_Avoid_: Middleware, interceptor, filter, decorator

**Pipeline order**:
The order in which pipeline behaviors wrap a handler. The first-registered behavior is
outermost and runs first. Registration order is the contract; discovery order is not.
_Avoid_: Priority, rank

**Result**:
The outcome of a request: either success or one or more errors. A result that carries a
response exposes it only on success; a failed result carries errors and no response.
_Avoid_: Response, outcome, reply

**Error**:
A machine-readable code paired with a human-readable message describing why a request
failed.
_Avoid_: Exception, fault, failure
