using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsService.Domain.NewsService.Domain.Exceptions;

public class AnotherAuthorEditNewsException(News news, Author author)
    : InvalidOperationException($"The user {author.Authorname} can't edit the {news.Title} note owned by the user  {news.Author.Authorname} (note id = {news.Id}).")
{
    public News News => news;
    public Author Author => author;
}