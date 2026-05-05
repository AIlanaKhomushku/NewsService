using NewsService.Domain.NewsService.Domain;
using NewsService.Domain.Repositories.Abstractions.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsService.Domain.Repositories.Abstractions;

public interface IAuthorRepository : IRepository<Author, Gui d>
{
    // Так как имя пользователя уникальное
    Task<Author?> GetAuthorByAuthornameAsync(string authorname, CancellationToken cancellationToken);
}