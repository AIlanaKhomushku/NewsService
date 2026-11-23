using AutoMapper;
using NewsService.Application.Models.Reaction;
using NewsService.Application.Services.Abstractions;
using NewsService.Domain.NewsService.Domain;
using NewsService.Domain.Repositories.Abstractions;

namespace NewsService.Application.Services;

public sealed class ReactionApplicationService(
    IReactionRepository reactionRepository,
    INewsRepository newsRepository,
    IUserRepository userRepository,
    IMapper mapper) : IReactionApplicationService
{
    public async Task<IEnumerable<ReactionModel>> GetModelsAsync(CancellationToken cancellationToken = default)
        => (await reactionRepository.GetAllAsync(cancellationToken, true)).Select(mapper.Map<ReactionModel>);

    public async Task<ReactionModel?> GetModelByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var reaction = await reactionRepository.GetByIdAsync(id, cancellationToken);
        return reaction is null ? null : mapper.Map<ReactionModel>(reaction);
    }

    public async Task<ReactionModel?> CreateModelAsync(CreateReactionModel information, CancellationToken cancellationToken = default)
    {
        var news = await newsRepository.GetByIdAsync(information.NewsId, cancellationToken);
        var user = await userRepository.GetByIdAsync(information.UserId, cancellationToken);
        if (news is null || user is null)
            return null;

        var previous = news.Reactions.FirstOrDefault(r => r.User.Id == user.Id);
        if (!news.SetReaction(user, information.Type, DateTime.UtcNow))
            return null;

        var existing = news.Reactions.FirstOrDefault(r => r.User.Id == user.Id);
        if (existing is null)
            return null;

        if (previous is null)
            await reactionRepository.AddAsync(existing, cancellationToken);
        else
            await reactionRepository.UpdateAsync(existing, cancellationToken);
        return mapper.Map<ReactionModel>(existing);
    }

    public async Task<bool> UpdateModelAsync(ReactionModel information, CancellationToken cancellationToken = default)
    {
        var reaction = await reactionRepository.GetByIdAsync(information.Id, cancellationToken);
        var user = reaction is null
            ? null
            : await userRepository.GetByIdAsync(reaction.User.Id, cancellationToken);
        if (reaction is null || user is null)
            return false;

        reaction.UpdateType(information.Type, user);
        return await reactionRepository.UpdateAsync(reaction, cancellationToken);
    }

    public async Task<bool> DeleteModelAsync(Guid id, CancellationToken cancellationToken = default)
        => await reactionRepository.DeleteAsync(id, cancellationToken);
}
