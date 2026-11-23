using AutoMapper;
using NewsService.Application.Models.Comment;
using NewsService.Application.Services.Abstractions;
using NewsService.Domain.NewsService.Domain;
using NewsService.Domain.NewsService.ValueObjects;
using NewsService.Domain.Repositories.Abstractions;

namespace NewsService.Application.Services;

public sealed class CommentApplicationService(
    ICommentRepository commentRepository,
    INewsRepository newsRepository,
    IUserRepository userRepository,
    IMapper mapper) : ICommentApplicationService
{
    public async Task<IEnumerable<CommentModel>> GetModelsAsync(CancellationToken cancellationToken = default)
        => (await commentRepository.GetAllAsync(cancellationToken, true)).Select(mapper.Map<CommentModel>);

    public async Task<CommentModel?> GetModelByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var comment = await commentRepository.GetByIdAsync(id, cancellationToken);
        return comment is null ? null : mapper.Map<CommentModel>(comment);
    }

    public async Task<CommentModel?> CreateModelAsync(CreateCommentModel information, CancellationToken cancellationToken = default)
    {
        var news = await newsRepository.GetByIdAsync(information.NewsId, cancellationToken);
        var user = await userRepository.GetByIdAsync(information.UserId, cancellationToken);
        if (news is null || user is null)
            return null;

        var comment = new Comment(news, user, new CommentText(information.Content), DateTime.UtcNow);
        if (!news.SetComment(comment))
            return null;

        await commentRepository.AddAsync(comment, cancellationToken);
        return mapper.Map<CommentModel>(comment);
    }

    public async Task<bool> UpdateModelAsync(CommentModel information, CancellationToken cancellationToken = default)
    {
        var comment = await commentRepository.GetByIdAsync(information.Id, cancellationToken);
        if (comment is null)
            return false;

        if (!comment.SetContent(new CommentText(information.Content)))
            return false;

        return await commentRepository.UpdateAsync(comment, cancellationToken);
    }

    public async Task<bool> DeleteModelAsync(Guid id, CancellationToken cancellationToken = default)
        => await commentRepository.DeleteAsync(id, cancellationToken);
}
