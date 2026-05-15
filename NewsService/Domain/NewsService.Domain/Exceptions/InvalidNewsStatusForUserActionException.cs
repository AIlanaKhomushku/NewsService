using NewsService.Domain.NewsService.Domain.Enums;

namespace NewsService.Domain.NewsService.Domain.Exceptions;

public class InvalidNewsStatusForUserActionException : Exception
{
    public InvalidNewsStatusForUserActionException(string action, NewsStatus status)
        : base($"Cannot perform {action} on news with status {status}. Only published news allow user actions.")
    {
    }
}