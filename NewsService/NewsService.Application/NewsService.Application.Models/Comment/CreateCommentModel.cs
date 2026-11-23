using NewsService.Application.Models.Base;

namespace NewsService.Application.Models.Comment;

public sealed class CreateCommentModel : ICreateModel
{
    public Guid NewsId { get; init; }
    public Guid UserId { get; init; }
    public string Content { get; init; } = string.Empty;
}