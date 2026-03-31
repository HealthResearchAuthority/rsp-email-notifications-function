namespace Rsp.NotifyFunction.UnitTests.Infrastructure;

public class ExceptionHandlingMiddlewareTests : TestServiceBase<ExceptionHandlingMiddleware>
{
    [Fact]
    public async Task Invoke_WhenNextDoesNotThrow_ShouldCallNext()
    {
        // Arrange
        var context = new Mock<FunctionContext>().Object;
        var nextCalled = false;

        Task Next(FunctionContext _)
        {
            nextCalled = true;
            return Task.CompletedTask;
        }

        // Act
        await Sut.Invoke(context, Next);

        // Assert
        nextCalled.ShouldBeTrue();
    }


    [Fact]
    public async Task Invoke_WhenNextThrows_ShouldLogError()
    {
        // Arrange
        var context = new Mock<FunctionContext>().Object;
        var thrownException = new InvalidOperationException("Something went wrong");

        Task Next(FunctionContext _)
        {
            throw thrownException;
        }

        // Act
        await Sut.Invoke(context, Next);

        // Assert
        Mocker.GetMock<ILogger<ExceptionHandlingMiddleware>>()
            .Invocations
            .Any()
            .ShouldBeTrue();
    }
}