using NewsService.Domain;
using NewsService.Domain.NewsService.Domain;
using NewsService.Domain.NewsService.Domain.Enums;
using NewsService.Domain.NewsService.ValueObjects;
using NewsService.ValueObjects;
namespace DomainApp;
internal class Program
{
    static void Main(string[] args)
    {
        var id = Guid.NewGuid();
        var name = new Authorname("Josh");
        var josh = new Author(id, name);

        var pit = new Author(id, new Authorname("pit"));

        var news = josh.CreateNews(new Title("Billie Eilish"), new Content("- she can sing"));
        var bella = new User(id, new Username("bella"));
        var mila = new User(Guid.NewGuid(), new Username("mila"));


        josh.UpdateNewsStatus(news, NewsStatus.Published);
        bella.ReactionNews(news, NewsReaction.Sad,DateTime.UtcNow);
        bella.CommentNews(news, new CommentText("i like her"),DateTime.Now);
        mila.CommentNews(news, new CommentText("i dont like her"), DateTime.Now);
        mila.ReactionNews(news, NewsReaction.Sad, DateTime.UtcNow);
        var news2 = pit.CreateNews(new Title("mommy"), new Content("yeaaaaah mooom bomb!!!"));
        Console.WriteLine(news.ToString());
        Console.WriteLine(news2.ToString());

    }
}
