using Microsoft.EntityFrameworkCore;
using NewsService.Domain.NewsService.Domain;
using NewsService.Domain.Repositories.Abstractions;
using NewsService.Infrastructure.EntityFramework;
using NewsService.Domain.NewsService.ValueObjects;

namespace NewsService.Infrastructure.EntityFramework.RepositoriesEF;

public class EfAuthorRepository(ApplicationDbContext context)
    : EfRepository<Author, Guid>(context), IAuthorRepository
{
    private readonly DbSet<Author> _authors = context.Set<Author>();

    public override Task<Author?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        => _authors.Include("_newss")
        .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

    public Task<Author?> GetAuthorByAuthornameAsync(string authorname, CancellationToken cancellationToken)
        => _authors.Include("_newss")
        .FirstOrDefaultAsync(a => a.Authorname.Equals(new Authorname(authorname)), cancellationToken);
}