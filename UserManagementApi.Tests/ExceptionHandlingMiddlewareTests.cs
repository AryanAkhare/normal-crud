using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;
using UserManagementApi.Middleware;
using Xunit;

namespace UserManagementApi.Tests;

public sealed class ExceptionHandlingMiddlewareTests
{
    [Fact]
    public async Task InvokeAsync_ReturnsGenericProblemDetailsForUnhandledException()
    {
        var middleware = new ExceptionHandlingMiddleware(
            _ => throw new InvalidOperationException("Sensitive internal detail"),
            NullLogger<ExceptionHandlingMiddleware>.Instance);
        var context = new DefaultHttpContext();
        context.Request.Path = "/api/users";
        await using var responseBody = new MemoryStream();
        context.Response.Body = responseBody;

        await middleware.InvokeAsync(context);

        Assert.Equal(StatusCodes.Status500InternalServerError, context.Response.StatusCode);
        Assert.Equal("application/problem+json", context.Response.ContentType);

        responseBody.Position = 0;
        using var document = await JsonDocument.ParseAsync(responseBody);
        var response = document.RootElement;
        Assert.Equal("An unexpected error occurred.", response.GetProperty("title").GetString());
        Assert.False(response.TryGetProperty("detail", out _));
    }
}
