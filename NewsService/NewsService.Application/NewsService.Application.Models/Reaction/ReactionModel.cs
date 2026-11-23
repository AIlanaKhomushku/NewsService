using NewsService.Application.Models.Base;
using NewsService.ValueObjects;

namespace NewsService.Application.Models.Reaction;

public sealed class ReactionModel : IModel<Guid>
{
    public Guid Id { get; init; }
    public Guid NewsId { get; init; }
    public Guid UserId { get; init; }
    public NewsReaction Type { get; init; }
    public DateTime CreationDate { get; init; }
}