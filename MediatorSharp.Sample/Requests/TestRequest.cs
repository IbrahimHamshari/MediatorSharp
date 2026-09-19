using MediatorSharp;
using MediatorSharp.Sample.Models;

namespace MediatorSharp.Sample.Requests;

public record TestRequest(int Id) : IRequest<Test>;
