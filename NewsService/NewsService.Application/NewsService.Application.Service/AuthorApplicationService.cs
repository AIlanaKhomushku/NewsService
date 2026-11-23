using AutoMapper;
using NewsService.Application.Models.Author;
using NewsService.Application.Services.Abstractions;
using NewsService.Domain.NewsService.Domain;
using NewsService.Domain.NewsService.ValueObjects;
using NewsService.Domain.Repositories.Abstractions;

namespace NewsService.Application.Services;

public sealed class AuthorApplicationService(
    IAuthorRepository repository,
    IMapper mapper) : IAuthorApplicationService
{
    public async Task<IEnumerable<AuthorModel>> GetModelsAsync(CancellationToken cancellationToken = default)
        => (await repository.GetAllAsync(cancellationToken))
            .Select(mapper.Map<AuthorModel>);

    public async Task<AuthorModel?> GetModelByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var author = await repository.GetByIdAsync(id, cancellationToken);
        return author is null ? null : mapper.Map<AuthorModel>(author);
    }

    public async Task<AuthorModel?> GetAuthorByUsernameAsync(
        string userName,
        CancellationToken cancellationToken = default)
    {
        var author = await repository.GetAuthorByAuthornameAsync(userName, cancellationToken);
        return author is null ? null : mapper.Map<AuthorModel>(author);
    }

    public async Task<AuthorModel?> CreateModelAsync(
        CreateAuthorModel authorInformation,
        CancellationToken cancellationToken = default)
    {
        if (await GetAuthorByUsernameAsync(authorInformation.Authorname, cancellationToken) is not null)
            return null;

        var author = new Author(Guid.NewGuid(), new Authorname(authorInformation.Authorname));
        await repository.AddAsync(author, cancellationToken);
        return mapper.Map<AuthorModel>(author);
    }

    public async Task<bool> UpdateModelAsync(
        AuthorModel authorInformation,
        CancellationToken cancellationToken = default)
    {
        var author = await repository.GetByIdAsync(authorInformation.Id, cancellationToken);
        if (author is null)
            return false;

        var existing = await repository.GetAuthorByAuthornameAsync(
            authorInformation.Authorname,
            cancellationToken);
        if (existing is not null && existing.Id != author.Id)
            return false;

        if (!author.ChangeAuthorname(new Authorname(authorInformation.Authorname)))
            return false;

        return await repository.UpdateAsync(author, cancellationToken);
    }

    public async Task<bool> DeleteModelAsync(Guid id, CancellationToken cancellationToken = default)
        => await repository.DeleteAsync(id, cancellationToken);
}