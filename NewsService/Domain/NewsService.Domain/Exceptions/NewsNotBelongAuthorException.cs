using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsService.Domain.NewsService.Domain.Exceptions;

public class NewsNotBelongAuthorException(News news, Author author)
: InvalidOperationException($"The news {news.Title} is not in the author's news sequence (author {author.Authorname}, news id = {news.Id}).")
{
    public News News => news;
    public Author Author => author;
}
