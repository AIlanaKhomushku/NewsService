using NewsService.Domain.NewsService.Domain.Base;
using NewsService.ValueObjects;
using System;

namespace NewsService.Domain.NewsService.Domain;

public class Reaction : Entity<Guid>
{
    public Guid NewsId { get; private set; }
    public Guid UserId { get; private set; }
    public News News { get; private set; } = default!;
    public User User { get; private set; } = default!;
    public NewsReaction Type { get; private set; }
    public DateTime CreationDate { get; private set; }

    public Reaction(News news, User user, NewsReaction type)
        : this(Guid.NewGuid(), news, user, type, DateTime.UtcNow)
    { }

    protected Reaction(Guid id, News news, User user, NewsReaction type, DateTime creationDate)
        : base(id)
    {
        News = news ?? throw new ArgumentNullException(nameof(news));
        User = user ?? throw new ArgumentNullException(nameof(user));
        NewsId = news.Id;
        UserId = user.Id;
        Type = type;
        CreationDate = creationDate;
    }

    private Reaction() { }

    internal void UpdateType(NewsReaction newType)
    {
        Type = newType;
        CreationDate = DateTime.UtcNow;
    }
}