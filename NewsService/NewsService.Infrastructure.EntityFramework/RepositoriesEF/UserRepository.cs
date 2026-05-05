using Microsoft.EntityFrameworkCore;
using NewsService.Domain.NewsService.Domain;
using NewsService.Domain.Repositories.Abstractions;
using NewsService.Infrastructure.EntityFramework;
using NewsService.Domain.NewsService.ValueObjects;


namespace NewsService.Infrastructure.EntityFramework.RepositoriesEF;

public class EfUserRepository(ApplicationDbContext context)
    : EfRepository<User, Guid>(context), IUserRepository
{
    private readonly DbSet<User> _users = context.Set<User>();

    public override Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        => _users.Include("_comments")
        .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

    public Task<User?> GetUserByUsernameAsync(string username, CancellationToken cancellationToken)
        => _users.Include("_comments")
        .FirstOrDefaultAsync(u => u.Username.Equals(new Username(username)), cancellationToken);
}
