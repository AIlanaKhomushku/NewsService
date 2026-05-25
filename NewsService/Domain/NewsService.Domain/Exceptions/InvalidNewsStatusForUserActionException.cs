using NewsService.Domain.NewsService.Domain.Enums;

namespace NewsService.Domain.NewsService.Domain.Exceptions;


    public class InvalidNewsStatusForUserActionException(News news, NewsStatus status)
        : InvalidOperationException($"Cannot perform on news with status {status}. Only published news allow user actions.")
    {
    public News News => news;
}
