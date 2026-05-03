using NewsService.Domain.NewsService.Domain;
using NewsService.Domain.Repositories.Abstractions.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsService.Domain.Repositories.Abstractions;

public interface IAuthorRepository : IRepository<User, Guid>
{
    // Так как имя пользователя уникальное
    Task<Author?> GetUserByAuthornameAsync(string Authorname, CancellationToken cancellationToken);
}