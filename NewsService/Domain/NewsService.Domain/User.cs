using NewsService.Domain.NewsService.Domain.Base;
using NewsService.Domain.NewsService.Domain.Enums;
using NewsService.Domain.NewsService.Domain.Exceptions;
using NewsService.Domain.NewsService.ValueObjects;

namespace NewsService.Domain.NewsService.Domain;

public class User(Guid id, Username username) : Entity<Guid>(id)
{
    private readonly ICollection<Comment>_comments=[];
    public IReadOnlyCollection<Comment> Comments => _comments.ToList().AsReadOnly();
    
    public Username Username { get; private set; } = username ?? throw new ArgumentNullValueException(nameof(username));


    /// <summary> 
    /// Changes the user's username. 
    /// </summary>
    /// <param name="newUsername">New user's username.</param>
    internal bool ChangeUsername(Username newUsername)
    {
        if (newUsername == null) throw new ArgumentNullValueException(nameof(newUsername));

        if (Username == newUsername) return false;

        Username = newUsername;
        return true;
    }
    /// <summary> 
    /// ReactionNews the user's username. 
    /// </summary>
    /// <param name="newUsername">New user's username.</param>
    public bool ReactionNews(News news,NewsReaction newsReaction)
    {
        if(news == null) throw new ArgumentNullValueException(nameof(news));
        return news.SetReaction(this,newsReaction);
        
    }
    
   /// <summary>
   /// комментировать новости
   /// </summary>
   /// <param name="news"></param>
   /// <param name="newcontent"></param>
   /// <returns></returns>
   /// <exception cref="ArgumentNullValueException"></exception>
   /// <exception cref="ArgumentNullException"></exception>
    public bool CommentNews(News news,Content newcontent)
    {
        if (news == null) throw new ArgumentNullValueException(nameof(news));
        if(newcontent==null) throw new ArgumentNullException(nameof(newcontent));
        var comm=(new Comment(news, this, newcontent, DateTime.UtcNow));
        news.SetComment(comm);
        _comments.Add(comm);

        return true;
    }
    public override string ToString()
    {
        return $"{Username.ToString()}";
    }
}
