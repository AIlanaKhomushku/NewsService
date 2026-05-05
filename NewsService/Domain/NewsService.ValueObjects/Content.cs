using NewsService.Domain.NewsService.ValueObjects.Base;
using NewsService.Domain.NewsService.ValueObjects.Validators;

namespace NewsService.Domain.NewsService.ValueObjects;

/// <summary>
/// Represents type of the entity's сontent.
/// </summary>
/// <param name="content">The сontent of the entity.</param>
public class Content(string content) : ValueObject<string>(new ContentValidator(), content);
