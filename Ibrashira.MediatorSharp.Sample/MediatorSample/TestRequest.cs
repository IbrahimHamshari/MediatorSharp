using MediatorSharp.lib.Interfaces;
using MediatorSharp.Models;

namespace MediatorSharp.Mediator;

public record TestRequest(int Id) : IRequest<Test>;
