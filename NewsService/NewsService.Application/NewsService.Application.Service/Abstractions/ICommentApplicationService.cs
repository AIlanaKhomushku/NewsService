using NewsService.Application.Models.Comment;
using NewsService.Application.Services.Abstractions.Base;

namespace NewsService.Application.Services.Abstractions;

public interface ICommentApplicationService : IApplicationService<CommentModel, CreateCommentModel, Guid>
{
}
