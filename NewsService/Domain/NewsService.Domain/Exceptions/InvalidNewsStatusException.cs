using NewsService.Domain.NewsService.Domain.Enums;

namespace NewsService.Domain.NewsService.Domain.Exceptions;

public class InvalidNewsStatusException : Exception
{
    public InvalidNewsStatusException(News news, NewsStatus status)
        : base($"This news {news.Id} already and status is {status}")
    {
    }
}