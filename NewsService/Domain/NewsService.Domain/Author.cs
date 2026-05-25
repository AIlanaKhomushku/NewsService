using NewsService.Domain.NewsService.Domain.Base;
using NewsService.Domain.NewsService.Domain.Enums;
using NewsService.Domain.NewsService.Domain.Exceptions;
using NewsService.Domain.NewsService.ValueObjects;
using NewsService.ValueObjects;

namespace NewsService.Domain.NewsService.Domain;

public class Author(Guid id, Authorname authorname) : Entity<Guid>(id)
{

    private readonly ICollection<News> _newss = [];

    public IReadOnlyCollection<News> PublishedNews => _newss.Where(a => a.NewsStatus == NewsStatus.Published).ToList().AsReadOnly();

    public Authorname Authorname { get; private set; } = authorname ?? throw new ArgumentNullValueException(nameof(authorname));

    /// <summary>
    /// поменять Authorname
    /// </summary>
    /// <param name="newAuthorname"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullValueException"></exception>
    internal bool ChangeAuthorname(Authorname newAuthorname)
    {
        if (newAuthorname == null) throw new ArgumentNullValueException(nameof(newAuthorname));

        if (Authorname == newAuthorname) return false;

        Authorname = newAuthorname;
        return true;
    }
    /// <summary>
    /// создать новость
    /// </summary>
    /// <param name="title"></param>
    /// <param name="content"></param>
    /// <returns></returns>
    public News CreateNews(Title title, Content content)
    {
       if(title == null) throw new ArgumentNullValueException(nameof(title));
         if(content == null) throw new ArgumentNullValueException(nameof(content));
        var news = new News(title, content, this, DateTime.UtcNow);
        if (_newss.Contains(news)) throw new NewsNotBelongAuthorException(news, this);
        _newss.Add(news);
        
        return news;
    }
    /// <summary>
    /// редактировать новость
    /// </summary>
    /// <param name="news"></param>
    /// <param name="title"></param>
    /// <param name="content"></param>
    /// <returns></returns>
    /// <exception cref="AnotherAuthorEditNewsException"></exception>
    /// <exception cref="NewsNotBelongAuthorException"></exception>
    public bool EditNews(News news, Title title, Content content)
    {
        if (news.Author != this) throw new AnotherAuthorEditNewsException(news, this);
        if (!_newss.Contains(news)) throw new NewsNotBelongAuthorException(news, this);
        if(news.NewsStatus != NewsStatus.Created)
            throw new InvalidNewsStatusException(news,news.NewsStatus);
        var isChangeTitle = news.SetTitle(title,this);
        var isChangeContent = news.SetContent(content,this);
        var isEdit = isChangeTitle || isChangeContent;
        if (isEdit) { news.SetModificationData(DateTime.UtcNow,this); }
        return isEdit;
    }
    /// <summary>
    /// удалить новость
    /// </summary>
    /// <param name="news"></param>
    /// <exception cref="ArgumentNullValueException"></exception>
    /// <exception cref="AnotherAuthorDeleteNewsException"></exception>
    /// <exception cref="NewsNotBelongAuthorException"></exception>
    public void DeleteNews(News news)
    {
        if (news == null) throw new ArgumentNullValueException(nameof(news));
        if (news.Author != this) throw new AnotherAuthorDeleteNewsException(news, this);
        if (!_newss.Contains(news)) throw new NewsNotBelongAuthorException(news, this);
        if(news.NewsStatus == NewsStatus.Deleted)
            throw new InvalidNewsStatusException(news,news.NewsStatus);

        _newss.Remove(news);
    }
    /// <summary>
    /// обновить статус новости
    /// </summary>
    /// <param name="news"></param>
    /// <param name="newStatus"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullValueException"></exception>
    /// <exception cref="AnotherAuthorEditNewsException"></exception>
    /// <exception cref="NewsNotBelongAuthorException"></exception>
    public bool UpdateNewsStatus(News news, NewsStatus newStatus)
    {
        if (news is null)
            throw new ArgumentNullValueException(nameof(news));

        if (news.Author != this)
            throw new AnotherAuthorEditNewsException(news, this);

        if (!_newss.Contains(news))
            throw new NewsNotBelongAuthorException(news, this);
        if(news.NewsStatus==NewsStatus.Deleted)
            throw new InvalidNewsStatusException(news, news.NewsStatus);

        return news.SetStatus(this,newStatus);
    }
}