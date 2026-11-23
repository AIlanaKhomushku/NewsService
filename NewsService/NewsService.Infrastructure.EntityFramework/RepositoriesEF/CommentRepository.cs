using Microsoft.EntityFrameworkCore;
using NewsService.Domain.NewsService.Domain;
using NewsService.Domain.Repositories.Abstractions;

namespace NewsService.Infrastructure.EntityFramework.RepositoriesEF;

public class EfCommentRepository(ApplicationDbContext context)
    : EfRepository<Comment, Guid>(context), ICommentRepository
{
    private readonly DbSet<Comment> _comments = context.Set<Comment>();

    public override async Task<IEnumerable<Comment>> GetAllAsync(
        CancellationToken cancellationToken,
        bool asNoTracking = false)
    {
        var query = asNoTracking ? _comments.AsNoTracking() : _comments;
        return await query
            .Include(comment => comment.News)
            .Include(comment => comment.User)
            .ToListAsync(cancellationToken);
    }

    public override Task<Comment?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        => _comments
            .Include(c => c.News)
            .Include(c => c.User)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
}
