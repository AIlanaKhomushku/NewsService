
using NewsService.Domain.NewsService.ValueObjects.Base;
using NewsService.Domain.NewsService.ValueObjects.Validators;

namespace NewsService.Domain.NewsService.ValueObjects;

/// <summary>
/// Represents type of the entity's authorname.
/// </summary>
/// <param name="name">The authorname of the entity.</param>
public class Authorname(string name) : ValueObject<string>(new AuthornameValidator(), name);
