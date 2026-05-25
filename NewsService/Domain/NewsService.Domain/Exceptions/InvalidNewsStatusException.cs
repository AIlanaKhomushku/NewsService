using NewsService.Domain.NewsService.Domain.Enums;

namespace NewsService.Domain.NewsService.Domain.Exceptions;



    public class InvalidNewsStatusException(News news, NewsStatus status)
        : InvalidOperationException($"This news {news.Id} already and status is {status}")
    {
               public News News => news;
}
