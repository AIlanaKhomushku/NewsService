using NewsService.Application.Models.Base;

namespace NewsService.Application.Models.Comment;

public sealed class CommentModel : IModel<Guid>
{
    public Guid Id { get; init; }
    public Guid NewsId { get; init; }
    public Guid UserId { get; init; }
    public string Content { get; init; } = string.Empty;
    public DateTime CreationData { get; init; }
}