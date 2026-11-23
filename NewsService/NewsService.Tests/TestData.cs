using AutoMapper;
using Microsoft.Extensions.Logging.Abstractions;
using NewsService.Application.Services.Mapping;
using NewsService.Domain.NewsService.Domain;
using NewsService.Domain.NewsService.Domain.Enums;
using NewsService.Domain.NewsService.ValueObjects;
using NewsService.ValueObjects;

namespace NewsService.Tests;

internal static class TestData
{
    public static IMapper CreateMapper()
    {
        var configuration = new MapperConfiguration(
            expression => expression.AddProfile<ApplicationProfile>(),
            NullLoggerFactory.Instance);
        return configuration.CreateMapper();
    }

    public static Author CreateAuthor(Guid? id = null, string name = "author")
        => new(id ?? Guid.NewGuid(), new Authorname(name));

    public static User CreateUser(Guid? id = null, string name = "user")
        => new(id ?? Guid.NewGuid(), new Username(name));

    public static News CreateNews(Author? author = null, string title = "title", string content = "content")
    {
        author ??= CreateAuthor();
        return author.CreateNews(new Title(title), new Content(content));
    }

    public static News CreatePublishedNews(Author? author = null)
    {
        author ??= CreateAuthor();
        var news = CreateNews(author);
        author.UpdateNewsStatus(news, NewsStatus.Published);
        return news;
    }
}
