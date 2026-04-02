using NewsService.Domain.NewsService.ValueObjects.Base;
using NewsService.Domain.NewsService.ValueObjects.Validators;

namespace NewsService.Domain.NewsService.ValueObjects;

/// <summary>
/// Represents type of the entity's title.
/// </summary>
/// <param name="title">The title of the entity.</param>
public class Title(string title) : ValueObject<string>(new TitleValidator(), title);
