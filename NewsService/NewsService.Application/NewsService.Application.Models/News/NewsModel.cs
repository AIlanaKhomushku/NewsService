using NewsService.Application.Models.Base;
using NewsService.Domain.NewsService.Domain.Enums;

namespace NewsService.Application.Models.News;

public sealed class NewsModel : IModel<Guid>
{
    public Guid Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Content { get; init; } = string.Empty;
    public DateTime CreationData { get; init; }
    public DateTime? ModificationData { get; init; }
    public NewsStatus NewsStatus { get; init; }
    public Guid AuthorId { get; init; }
}