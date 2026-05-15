
using NewsService.Domain.NewsService.ValueObjects.Base;
using NewsService.Domain.NewsService.ValueObjects.Validators;

namespace NewsService.Domain.NewsService.ValueObjects;

/// <summary>
/// Represents type of the entity's username.
/// </summary>
/// <param name="name">The username of the entity.</param>
public class Username(string name) : ValueObject<string>(new UsernameValidator(), name);
