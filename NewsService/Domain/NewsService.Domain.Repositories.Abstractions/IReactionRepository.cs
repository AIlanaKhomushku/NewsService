using NewsService.Domain.NewsService.Domain;
using NewsService.Domain.Repositories.Abstractions.Base;

namespace NewsService.Domain.Repositories.Abstractions;

public interface IReactionRepository : IRepository<Reaction, Guid>
{
}
