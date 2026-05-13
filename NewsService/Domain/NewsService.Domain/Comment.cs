using NewsService.Domain.NewsService.Domain.Base;
using NewsService.Domain.NewsService.Domain.Exceptions;
using NewsService.Domain.NewsService.ValueObjects;
using NewsService.ValueObjects;
namespace NewsService.Domain.NewsService.Domain;

public class Comment : Entity<Guid>
{

    public News News { get; } = default!;
    public User User { get; } = default!;
    public CommentText Content { get; private set; }
    public DateTime CreationData { get; }


    public Comment(
        News news,
        User user,
        CommentText content,
        DateTime creationData
        )
        : this(Guid.NewGuid(), news, user, content, creationData) { }

    private Comment() { }

    protected Comment(
        Guid id,
        News news,
        User user,
        CommentText content,
        DateTime creationData = default
        )
        : base(id)
    {

        News = news ?? throw new ArgumentNullValueException(nameof(news));
        User = user ?? throw new ArgumentNullValueException(nameof(user));

        Content = content ?? throw new ArgumentNullValueException(nameof(content));

        CreationData = creationData;

    }
    /// <summary>
    /// добавляем содержимое новости
    /// </summary>
    /// <param name="newContent"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullValueException"></exception>
    public bool SetContent(CommentText newContent)
    {
        if (newContent == null) { throw new ArgumentNullValueException(nameof(newContent)); }
        if (Content == newContent)
        {
            return false;
        }
        Content = newContent;
        return true;
    }
    public override string ToString()
    {
        return Content.ToString();
    }

}