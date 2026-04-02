using NewsService.Domain;
using NewsService.Domain.NewsService.Domain;
using NewsService.Domain.NewsService.ValueObjects;
using NewsService.Domain.NewsService.Domain.Enums;
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
        bella.ReactionNews(news, NewsReaction.Sad);

        josh.UpdateNewsStatus(news, NewsStatus.Deleted);

        bella.CommentNews(news, new Content("i like her"));
        //bella.ReactionNews(news, NewsReaction.Sad);
        mila.CommentNews(news, new Content("i dont like her"));
        mila.ReactionNews(news, NewsReaction.Laugh);
        var news2 = pit.CreateNews(new Title("mommy"), new Content("yeaaaaah mooom bomb!!!"));
        Console.WriteLine(news.ToString());

    }
}
