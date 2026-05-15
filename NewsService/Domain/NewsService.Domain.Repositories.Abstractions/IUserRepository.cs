using NewsService.Domain.NewsService.Domain;
using NewsService.Domain.Repositories.Abstractions.Base;


namespace NewsService.Domain.Repositories.Abstractions;

public interface IUserRepository : IRepository<User, Guid>
{
    // Так как имя пользователя уникальное
    Task<User?> GetUserByUsernameAsync(string username, CancellationToken cancellationToken);
}