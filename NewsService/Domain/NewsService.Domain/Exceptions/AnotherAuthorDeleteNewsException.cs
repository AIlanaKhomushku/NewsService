
namespace NewsService.Domain.NewsService.Domain.Exceptions;

public class AnotherAuthorDeleteNewsException(News news, Author author)
: InvalidOperationException($"The author {author.Authorname} can't delete the {news.Title} news owned by the author  {news.Author.Authorname} (news id = {news.Id}).")
{
    public News News => news;
    public Author Author => author;
}
