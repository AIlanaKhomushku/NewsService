using NewsService.Application.Models.Base;

namespace NewsService.Application.Models.Author;

public sealed class AuthorModel : IModel<Guid>
{
    public Guid Id { get; init; }
    public string Authorname { get; init; } = string.Empty;
}