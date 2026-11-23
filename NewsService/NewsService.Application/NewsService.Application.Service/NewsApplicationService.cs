using AutoMapper;
using NewsService.Application.Models.News;
using NewsService.Application.Services.Abstractions;
using NewsService.Domain.NewsService.Domain.Enums;
using NewsService.Domain.NewsService.ValueObjects;
using NewsService.Domain.Repositories.Abstractions;

namespace NewsService.Application.Services;

public sealed class NewsApplicationService(
    INewsRepository newsRepository,
    IAuthorRepository authorRepository,
    IMapper mapper) : INewsApplicationService
{
    public async Task<IEnumerable<NewsModel>> GetModelsAsync(CancellationToken cancellationToken = default)
        => (await newsRepository.GetAllAsync(cancellationToken, true)).Select(mapper.Map<NewsModel>);

    public async Task<NewsModel?> GetModelByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var news = await newsRepository.GetByIdAsync(id, cancellationToken);
        return news is null ? null : mapper.Map<NewsModel>(news);
    }

    public async Task<NewsModel?> CreateModelAsync(CreateNewsModel information, CancellationToken cancellationToken = default)
    {
        var author = await authorRepository.GetByIdAsync(information.AuthorId, cancellationToken);
        if (author is null)
            return null;

        var news = author.CreateNews(new Title(information.Title), new Content(information.Content));
        await newsRepository.AddAsync(news, cancellationToken);
        return mapper.Map<NewsModel>(news);
    }

    public async Task<bool> UpdateModelAsync(NewsModel information, CancellationToken cancellationToken = default)
    {
        var news = await newsRepository.GetByIdAsync(information.Id, cancellationToken);
        var author = await authorRepository.GetByIdAsync(information.AuthorId, cancellationToken);
        if (news is null || author is null)
            return false;

        var changed = author.EditNews(news, new Title(information.Title), new Content(information.Content));
        return changed && await newsRepository.UpdateAsync(news, cancellationToken);
    }

    public async Task<bool> UpdateStatusAsync(Guid id, Guid authorId, NewsStatus status, CancellationToken cancellationToken = default)
    {
        var news = await newsRepository.GetByIdAsync(id, cancellationToken);
        var author = await authorRepository.GetByIdAsync(authorId, cancellationToken);
        if (news is null || author is null)
            return false;

        var changed = author.UpdateNewsStatus(news, status);
        return changed && await newsRepository.UpdateAsync(news, cancellationToken);
    }

    public async Task<bool> DeleteModelAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var news = await newsRepository.GetByIdAsync(id, cancellationToken);
        if (news is null)
            return false;

        var author = await authorRepository.GetByIdAsync(news.Author.Id, cancellationToken);
        if (author is null)
            return false;

        author.DeleteNews(news);
        return await newsRepository.DeleteAsync(news, cancellationToken);
    }
}
