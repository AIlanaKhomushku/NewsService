using Microsoft.EntityFrameworkCore;
using NewsService.Domain.NewsService.Domain;
using NewsService.Domain.Repositories.Abstractions;

namespace NewsService.Infrastructure.EntityFramework.RepositoriesEF;

public class EfReactionRepository(ApplicationDbContext context)
    : EfRepository<Reaction, Guid>(context), IReactionRepository
{
    private readonly DbSet<Reaction> _reactions = context.Set<Reaction>();

    public override async Task<IEnumerable<Reaction>> GetAllAsync(
        CancellationToken cancellationToken,
        bool asNoTracking = false)
    {
        var query = asNoTracking ? _reactions.AsNoTracking() : _reactions;
        return await query
            .Include(reaction => reaction.News)
            .Include(reaction => reaction.User)
            .ToListAsync(cancellationToken);
    }

    public override Task<Reaction?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        => _reactions
            .Include(r => r.News)
            .Include(r => r.User)
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
}
