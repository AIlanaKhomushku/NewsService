using NewsService.Application.Models.Reaction;
using NewsService.Application.Services.Abstractions.Base;

namespace NewsService.Application.Services.Abstractions;

public interface IReactionApplicationService : IApplicationService<ReactionModel, CreateReactionModel, Guid>
{
}
