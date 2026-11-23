using System.Text.Json;
using Microsoft.AspNetCore.Http;
using NewsService.Domain.NewsService.Domain.Enums;
using NewsService.Domain.NewsService.Domain.Exceptions;
using NewsService.WebHost.Helpers;

namespace NewsService.Tests;

public sealed class WebHostHelperTests
{
    [Fact]
    public async Task Invalid_news_status_handler_returns_conflict_problem_details()
    {
        var news = TestData.CreateNews();
        var context = new DefaultHttpContext();
        context.Request.Path = "/api/Comments";
        await using var responseBody = new MemoryStream();
        context.Response.Body = responseBody;
        var handler = new InvalidNewsStatusExceptionHandler();

        var handled = await handler.TryHandleAsync(
            context,
            new InvalidNewsStatusForUserActionException(news, NewsStatus.Created),
            CancellationToken.None);

        Assert.True(handled);
        Assert.Equal(StatusCodes.Status409Conflict, context.Response.StatusCode);

        responseBody.Position = 0;
        using var document = await JsonDocument.ParseAsync(responseBody);
        Assert.Equal("News is not published", document.RootElement.GetProperty("title").GetString());
        Assert.Equal("/api/Comments", document.RootElement.GetProperty("instance").GetString());
    }

    [Fact]
    public async Task Invalid_news_status_handler_does_not_handle_unrelated_exceptions()
    {
        var context = new DefaultHttpContext();
        var handler = new InvalidNewsStatusExceptionHandler();

        var handled = await handler.TryHandleAsync(
            context,
            new ArgumentException("invalid input"),
            CancellationToken.None);

        Assert.False(handled);
        Assert.Equal(StatusCodes.Status200OK, context.Response.StatusCode);
    }

    [Fact]
    public void AutoMapper_profile_is_valid()
    {
        var mapper = TestData.CreateMapper();

        mapper.ConfigurationProvider.AssertConfigurationIsValid();
    }
}
