using NewsService.Application.Models.Base;
using NewsService.ValueObjects;

namespace NewsService.Application.Models.Reaction;

public sealed class CreateReactionModel : ICreateModel
{
    public Guid NewsId { get; init; }
    public Guid UserId { get; init; }
    public NewsReaction Type { get; init; }
}