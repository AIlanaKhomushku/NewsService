using NewsService.Domain.NewsService.Domain;
using NewsService.Domain.Repositories.Abstractions.Base;


namespace NewsService.Domain.Repositories.Abstractions;

public interface IAuthorRepository : IRepository<Author, Guid>
{
    Task<Author?> GetAuthorByAuthornameAsync(string authorname, CancellationToken cancellationToken);
}