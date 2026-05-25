

namespace NewsService.Domain.NewsService.Domain.Exceptions;

public class InvalidModificationDataException(News News, DateTime modificationData)
: ArgumentException($"The modification time {modificationData} is invalid for news id{News.Id}")
{
    public News News => News;
    public DateTime ModificationData => modificationData;
}
