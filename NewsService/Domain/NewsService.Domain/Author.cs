using NewsService.Domain.NewsService.Domain.Base;
using NewsService.Domain.NewsService.Domain.Exceptions;
using NewsService.Domain.NewsService.ValueObjects;
using NewsService.Domain.NewsService.Domain.Enums;

namespace NewsService.Domain.NewsService.Domain;

public class Author(Guid id, Authorname authorname) : Entity<Guid>(id)
{

    private readonly ICollection<News> _newss = [];

    public IReadOnlyCollection<News>newss=>_newss.Where(a=>a.NewsStatus==NewsStatus.Published).ToList().AsReadOnly();
    
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
    public News CreateNews(Title title,Content content)
    {
        var news=new News( title, content, this, DateTime.UtcNow);
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
        if(news.Author !=this) throw new AnotherAuthorEditNewsException(news,this);
        if(!_newss.Contains(news)) throw new NewsNotBelongAuthorException(news,this);
        var isChangeTitle =news.SetTitle(title);
        var isChangeContent=news.SetContent(content);
        var isEdit=isChangeTitle||isChangeContent;
        if(isEdit) {news.SetModificationData(DateTime.UtcNow);}
        return isEdit;
    }
    /// <summary>
    /// удалить новость
    /// </summary>
    /// <param name="news"></param>
    /// <exception cref="ArgumentNullValueException"></exception>
    /// <exception cref="AnotherUserDeleteNewsException"></exception>
    /// <exception cref="NewsNotBelongAuthorException"></exception>
    public void DeleteNews(News news)
    {
        if(news==null) throw new ArgumentNullValueException(nameof(news)); 
        if(news.Author!=this)throw new AnotherUserDeleteNewsException(news,this);
        if (!_newss.Contains(news)) throw new NewsNotBelongAuthorException(news, this);

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
        if (news == null)
            throw new ArgumentNullValueException(nameof(news));

        if (news.Author != this)
            throw new AnotherAuthorEditNewsException(news, this);

        if (!_newss.Contains(news))
            throw new NewsNotBelongAuthorException(news, this);

        return news.SetStatus(newStatus);
    }
}
