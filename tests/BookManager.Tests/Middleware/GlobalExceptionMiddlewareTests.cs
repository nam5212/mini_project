using System.Net;
using BookManager.API.Middleware;
using Microsoft.Extensions.Logging;

namespace BookManager.Tests.Middleware;

public class GlobalExceptionMiddlewareTests
{
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddlewareTests()
    {
        _logger = Substitute.For<ILogger<GlobalExceptionMiddleware>>();
    }

    [Fact]
    public async Task InvokeAsync_ShouldCallNext_WhenNoExceptionOccurs()
    {
        // Arrange
        var next = Substitute.For<RequestDelegate>();
        var middleware = new GlobalExceptionMiddleware(next, _logger);
        var context = new DefaultHttpContext();

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        await next.Received(1).Invoke(context);
        context.Response.StatusCode.Should().Be(200);
    }

    [Theory]
    [MemberData(nameof(GetExceptions))]
    public async Task InvokeAsync_ShouldHandleExceptionAndReturnExpectedStatusCode(Exception exception, HttpStatusCode expectedStatusCode, string expectedTitle)
    {
        // Arrange
        var next = Substitute.For<RequestDelegate>();
        next.When(x => x.Invoke(Arg.Any<HttpContext>())).Do(x => throw exception);

        var middleware = new GlobalExceptionMiddleware(next, _logger);
        var context = new DefaultHttpContext();
        context.Request.Path = "/api/test";
        context.Response.Body = new MemoryStream();

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        context.Response.StatusCode.Should().Be((int)expectedStatusCode);
        context.Response.ContentType.Should().Be("application/problem+json");

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var reader = new StreamReader(context.Response.Body);
        var responseBody = await reader.ReadToEndAsync();

        responseBody.Should().Contain(expectedTitle);
        responseBody.Should().Contain($"/api/test");
    }

    public static IEnumerable<object[]> GetExceptions()
    {
        yield return new object[] { new KeyNotFoundException("Item not found"), HttpStatusCode.NotFound, "Not Found" };
        yield return new object[] { new InvalidOperationException("Invalid operation"), HttpStatusCode.BadRequest, "Bad Request" };
        yield return new object[] { new ArgumentException("Invalid argument"), HttpStatusCode.BadRequest, "Bad Request" };
        yield return new object[] { new Exception("Unexpected crash"), HttpStatusCode.InternalServerError, "Internal Server Error" };
    }
}
