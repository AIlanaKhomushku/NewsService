using NewsService.Application.Models.Base;

namespace NewsService.Application.Models.News;

public sealed class CreateNewsModel : ICreateModel
{
    public Guid AuthorId { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Content { get; init; } = string.Empty;
}