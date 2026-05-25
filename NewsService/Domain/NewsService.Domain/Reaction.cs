using NewsService.Domain.NewsService.Domain.Base;
using NewsService.Domain.NewsService.Domain.Exceptions;
using NewsService.ValueObjects;
using System;

namespace NewsService.Domain.NewsService.Domain;

public class Reaction : Entity<Guid>
{

    public News News { get; private set; } = default!;
    public User User { get; private set; } = default!;
    public NewsReaction Type { get; private set; }
    public DateTime CreationDate { get; private set; }

    public Reaction(News news, User user, NewsReaction type,DateTime creationDate)
        : this(Guid.NewGuid(), news, user, type, creationDate)
    { }

    protected Reaction(Guid id, News news, User user, NewsReaction type, DateTime creationDate)
        : base(id)
    {
        News = news ?? throw new ArgumentNullException(nameof(news));
        User = user ?? throw new ArgumentNullException(nameof(user));

        Type = type;
        CreationDate = creationDate;
    }

    protected Reaction() { }

    internal void UpdateType(NewsReaction newType, User user)
    {
        if(User.Id != user.Id) throw new AnotherUserException(user, this);
        Type = newType;
        CreationDate = DateTime.UtcNow;
    }
}