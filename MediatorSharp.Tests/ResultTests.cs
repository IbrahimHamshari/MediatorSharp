using MediatorSharp.Tests.Fixtures;
using Microsoft.Extensions.DependencyInjection;

namespace MediatorSharp.Tests;

public class ResultTests
{
    [Fact]
    public void Result_Success_IsSuccessAndHasNoErrors()
    {
        var result = Result.Success;

        Assert.True(result.IsSuccess);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public void Result_FromError_RetainsErrorAndIsFailure()
    {
        var error = new TestError("E1", "boom");

        var result = Result.FromError(error);

        Assert.False(result.IsSuccess);
        Assert.Single(result.Errors);
        Assert.Same(error, result.Errors.Single());
    }

    [Fact]
    public void Result_ErrorConstructor_RetainsError()
    {
        var error = new TestError("E2", "boom");

        var result = new Result(error);

        Assert.False(result.IsSuccess);
        Assert.Contains(error, result.Errors);
    }

    [Fact]
    public void ResultGeneric_WithValue_IsSuccess()
    {
        var result = new Result<PongResponse>(new PongResponse { Value = "v" });

        Assert.True(result.IsSuccess);
        Assert.Equal("v", result.Value!.Value);
    }

    [Fact]
    public void ResultGeneric_FromError_ReturnsTypedResultAndRetainsError()
    {
        var error = new TestError("E3", "boom");

        var result = Result<PongResponse>.FromError(error);

        Assert.False(result.IsSuccess);
        Assert.Single(result.Errors);
        Assert.Same(error, result.Errors.Single());
        Assert.Null(result.Value);
    }

    [Fact]
    public void ResultGeneric_ErrorConstructor_RetainsErrorAndNullValue()
    {
        var error = new TestError("E4", "boom");

        var result = new Result<PongResponse>(error);

        Assert.False(result.IsSuccess);
        Assert.Contains(error, result.Errors);
        Assert.Null(result.Value);
    }

    [Fact]
    public void ResultGeneric_CollectionConstructor_RetainsAllErrors()
    {
        var errors = new List<IError>
        {
            new TestError("E5", "one"),
            new TestError("E6", "two"),
        };

        var result = new Result<PongResponse>(errors);

        Assert.False(result.IsSuccess);
        Assert.Equal(2, result.Errors.Count);
    }

    [Fact]
    public void ResultGeneric_ImplicitConversion_WrapsValue()
    {
        Result<PongResponse> result = new PongResponse { Value = "implicit" };

        Assert.True(result.IsSuccess);
        Assert.Equal("implicit", result.Value!.Value);
    }

    [Fact]
    public void Result_ImplementsInterfaces()
    {
        Assert.IsAssignableFrom<IResult>(Result.Success);
        Assert.IsAssignableFrom<IResult<PongResponse>>(new Result<PongResponse>(new PongResponse()));
    }
}
