using AutoMapper;
using NewsService.Application.Models.User;
using NewsService.Application.Services.Abstractions;
using NewsService.Domain.NewsService.Domain;
using NewsService.Domain.NewsService.ValueObjects;
using NewsService.Domain.Repositories.Abstractions;

namespace NewsService.Application.Services;

public sealed class UserApplicationService(
    IUserRepository repository,
    IMapper mapper) : IUserApplicationService
{
    public async Task<IEnumerable<UserModel>> GetModelsAsync(CancellationToken cancellationToken = default)
        => (await repository.GetAllAsync(cancellationToken, true)).Select(mapper.Map<UserModel>);

    public async Task<UserModel?> GetModelByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var user = await repository.GetByIdAsync(id, cancellationToken);
        return user is null ? null : mapper.Map<UserModel>(user);
    }

    public async Task<UserModel?> GetUserByUsernameAsync(string username, CancellationToken cancellationToken = default)
    {
        var user = await repository.GetUserByUsernameAsync(username, cancellationToken);
        return user is null ? null : mapper.Map<UserModel>(user);
    }

    public async Task<UserModel?> CreateModelAsync(CreateUserModel information, CancellationToken cancellationToken = default)
    {
        if (await GetUserByUsernameAsync(information.Username, cancellationToken) is not null)
            return null;

        var user = new User(Guid.NewGuid(), new Username(information.Username));
        await repository.AddAsync(user, cancellationToken);
        return mapper.Map<UserModel>(user);
    }

    public async Task<bool> UpdateModelAsync(UserModel information, CancellationToken cancellationToken = default)
    {
        var user = await repository.GetByIdAsync(information.Id, cancellationToken);
        if (user is null)
            return false;

        var existing = await repository.GetUserByUsernameAsync(information.Username, cancellationToken);
        if (existing is not null && existing.Id != user.Id)
            return false;

        if (!user.ChangeUsername(new Username(information.Username)))
            return false;

        return await repository.UpdateAsync(user, cancellationToken);
    }

    public async Task<bool> DeleteModelAsync(Guid id, CancellationToken cancellationToken = default)
        => await repository.DeleteAsync(id, cancellationToken);
}
