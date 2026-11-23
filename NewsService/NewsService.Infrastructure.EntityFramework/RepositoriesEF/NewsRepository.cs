using Microsoft.EntityFrameworkCore;
using NewsService.Domain.NewsService.Domain;
using NewsService.Domain.Repositories.Abstractions;

namespace NewsService.Infrastructure.EntityFramework.RepositoriesEF;

public class EfNewsRepository(ApplicationDbContext context)
    : EfRepository<News, Guid>(context), INewsRepository
{
    private readonly DbSet<News> _news = context.Set<News>();

    public override async Task<IEnumerable<News>> GetAllAsync(
        CancellationToken cancellationToken,
        bool asNoTracking = false)
    {
        var query = asNoTracking ? _news.AsNoTracking() : _news;
        return await query
            .Include(news => news.Author)
            .ToListAsync(cancellationToken);
    }

    public override Task<News?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        => _news
            .Include(n => n.Author)
            .Include(n => n.Reactions)
            .Include(n => n.Comments)
            .FirstOrDefaultAsync(n => n.Id == id, cancellationToken);
}
