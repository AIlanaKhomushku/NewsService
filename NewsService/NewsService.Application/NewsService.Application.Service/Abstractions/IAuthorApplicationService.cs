using NewsService.Application.Models.Author;
using NewsService.Application.Services.Abstractions.Base;

namespace NewsService.Application.Services.Abstractions;

public interface IAuthorApplicationService : IApplicationService<AuthorModel, CreateAuthorModel, Guid>
    {
    Task<AuthorModel?> GetAuthorByUsernameAsync(string userName, CancellationToken cancellationToken = default);
}