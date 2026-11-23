using NewsService.Application.Models.News;
using NewsService.Application.Services.Abstractions.Base;
using NewsService.Domain.NewsService.Domain.Enums;

namespace NewsService.Application.Services.Abstractions;

public interface INewsApplicationService : IApplicationService<NewsModel, CreateNewsModel, Guid>
{
    Task<bool> UpdateStatusAsync(Guid id, Guid authorId, NewsStatus status, CancellationToken cancellationToken = default);
}
