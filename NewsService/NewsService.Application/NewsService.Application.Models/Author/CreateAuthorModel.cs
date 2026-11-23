using NewsService.Application.Models.Base;

namespace NewsService.Application.Models.Author;

public sealed class CreateAuthorModel : ICreateModel
{
    public string Authorname { get; init; } = string.Empty;
}