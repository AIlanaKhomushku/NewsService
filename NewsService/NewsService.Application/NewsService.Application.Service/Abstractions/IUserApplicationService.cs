using NewsService.Application.Models.User;
using NewsService.Application.Services.Abstractions.Base;

namespace NewsService.Application.Services.Abstractions;

public interface IUserApplicationService : IApplicationService<UserModel, CreateUserModel, Guid>
{
    Task<UserModel?> GetUserByUsernameAsync(string username, CancellationToken cancellationToken = default);
}
