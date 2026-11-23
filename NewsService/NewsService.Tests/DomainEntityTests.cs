using NewsService.Domain.NewsService.Domain;
using NewsService.Domain.NewsService.Domain.Enums;
using NewsService.Domain.NewsService.Domain.Exceptions;
using NewsService.Domain.NewsService.ValueObjects;
using NewsService.ValueObjects;

namespace NewsService.Tests;

public sealed class DomainEntityTests
{
    [Fact]
    public void Author_changes_name_and_exposes_only_published_news()
    {
        var author = TestData.CreateAuthor();
        var draft = TestData.CreateNews(author);
        var published = TestData.CreateNews(author, "published title");

        Assert.False(author.ChangeAuthorname(new Authorname("author")));
        Assert.True(author.ChangeAuthorname(new Authorname("new-author")));
        Assert.Throws<ArgumentNullValueException>(() => author.ChangeAuthorname(null!));
        Assert.True(author.UpdateNewsStatus(published, NewsStatus.Published));

        Assert.Equal(new[] { published }, author.PublishedNews);
        Assert.DoesNotContain(draft, author.PublishedNews);
    }

    [Fact]
    public void Author_creates_news_and_rejects_missing_values()
    {
        var author = TestData.CreateAuthor();

        Assert.Throws<ArgumentNullValueException>(() => author.CreateNews(null!, new Content("content")));
        Assert.Throws<ArgumentNullValueException>(() => author.CreateNews(new Title("title"), null!));

        var news = author.CreateNews(new Title("title"), new Content("content"));
        Assert.Same(author, news.Author);
        Assert.Equal(NewsStatus.Created, news.NewsStatus);
    }

    [Fact]
    public void Author_edits_draft_only_when_content_changes()
    {
        var author = TestData.CreateAuthor();
        var news = TestData.CreateNews(author);

        Assert.False(author.EditNews(news, new Title("title"), new Content("content")));
        Assert.True(author.EditNews(news, new Title("new title"), new Content("new content")));
        Assert.Equal("new title", news.Title.Value);
        Assert.Equal("new content", news.Content.Value);
        Assert.NotNull(news.ModificationData);
    }

    [Fact]
    public void Author_rejects_edits_for_foreign_missing_or_published_news()
    {
        var author = TestData.CreateAuthor();
        var otherAuthor = TestData.CreateAuthor();
        var ownNews = TestData.CreateNews(author);
        var unattachedNews = new News(new Title("title"), new Content("content"), author, DateTime.UtcNow);
        var publishedNews = TestData.CreateNews(author);
        author.UpdateNewsStatus(publishedNews, NewsStatus.Published);

        Assert.Throws<AnotherAuthorEditNewsException>(() =>
            otherAuthor.EditNews(ownNews, new Title("updated"), new Content("updated")));
        Assert.Throws<NewsNotBelongAuthorException>(() =>
            author.EditNews(unattachedNews, new Title("updated"), new Content("updated")));
        Assert.Throws<InvalidNewsStatusException>(() =>
            author.EditNews(publishedNews, new Title("updated"), new Content("updated")));
    }

    [Fact]
    public void Author_deletes_news_and_rejects_invalid_delete_requests()
    {
        var author = TestData.CreateAuthor();
        var otherAuthor = TestData.CreateAuthor();
        var ownNews = TestData.CreateNews(author);
        var unattachedNews = new News(new Title("title"), new Content("content"), author, DateTime.UtcNow);
        var deletedNews = TestData.CreateNews(author);
        author.UpdateNewsStatus(deletedNews, NewsStatus.Deleted);

        Assert.Throws<ArgumentNullValueException>(() => author.DeleteNews(null!));
        Assert.Throws<AnotherAuthorDeleteNewsException>(() => otherAuthor.DeleteNews(ownNews));
        Assert.Throws<NewsNotBelongAuthorException>(() => author.DeleteNews(unattachedNews));
        Assert.Throws<InvalidNewsStatusException>(() => author.DeleteNews(deletedNews));

        author.DeleteNews(ownNews);
        Assert.DoesNotContain(ownNews, author.PublishedNews);
    }

    [Fact]
    public void Author_updates_status_and_rejects_invalid_news()
    {
        var author = TestData.CreateAuthor();
        var otherAuthor = TestData.CreateAuthor();
        var ownNews = TestData.CreateNews(author);
        var unattachedNews = new News(new Title("title"), new Content("content"), author, DateTime.UtcNow);
        var deletedNews = TestData.CreateNews(author);
        author.UpdateNewsStatus(deletedNews, NewsStatus.Deleted);

        Assert.Throws<ArgumentNullValueException>(() => author.UpdateNewsStatus(null!, NewsStatus.Published));
        Assert.Throws<AnotherAuthorEditNewsException>(() =>
            otherAuthor.UpdateNewsStatus(ownNews, NewsStatus.Published));
        Assert.Throws<NewsNotBelongAuthorException>(() =>
            author.UpdateNewsStatus(unattachedNews, NewsStatus.Published));
        Assert.Throws<InvalidNewsStatusException>(() =>
            author.UpdateNewsStatus(deletedNews, NewsStatus.Published));
        Assert.False(author.UpdateNewsStatus(ownNews, NewsStatus.Created));
        Assert.True(author.UpdateNewsStatus(ownNews, NewsStatus.Published));
    }

    [Fact]
    public void News_changes_title_and_content_only_for_its_author()
    {
        var author = TestData.CreateAuthor();
        var news = TestData.CreateNews(author);
        var otherAuthor = TestData.CreateAuthor();

        Assert.False(news.SetTitle(new Title("title"), author));
        Assert.True(news.SetTitle(new Title("updated"), author));
        Assert.False(news.SetContent(new Content("content"), author));
        Assert.True(news.SetContent(new Content("updated content"), author));
        Assert.Throws<ArgumentNullValueException>(() => news.SetTitle(null!, author));
        Assert.Throws<ArgumentNullValueException>(() => news.SetContent(null!, author));
        Assert.Throws<ArgumentNullValueException>(() => news.SetTitle(new Title("x"), null!));
        Assert.Throws<AnotherAuthorEditNewsException>(() => news.SetTitle(new Title("x"), otherAuthor));
        Assert.Throws<AnotherAuthorEditNewsException>(() => news.SetContent(new Content("new"), otherAuthor));
    }

    [Fact]
    public void News_modification_date_must_be_monotonic_and_not_precede_creation()
    {
        var author = TestData.CreateAuthor();
        var creation = DateTime.UtcNow;
        var news = new News(new Title("title"), new Content("content"), author, creation);
        var initialModification = creation.AddSeconds(1);

        Assert.Throws<ArgumentNullValueException>(() => news.SetModificationData(initialModification, null!));
        Assert.Throws<AnotherAuthorEditNewsException>(() =>
            news.SetModificationData(initialModification, TestData.CreateAuthor()));
        Assert.Throws<InvalidModificationDataException>(() =>
            news.SetModificationData(creation.AddTicks(-1), author));
        Assert.True(news.SetModificationData(initialModification, author));
        Assert.False(news.SetModificationData(initialModification, author));
        Assert.Throws<InvalidModificationDataException>(() =>
            news.SetModificationData(creation, author));
        Assert.True(news.SetModificationData(creation.AddSeconds(2), author));
    }

    [Fact]
    public void News_status_changes_only_for_its_author()
    {
        var author = TestData.CreateAuthor();
        var news = TestData.CreateNews(author);

        Assert.Throws<ArgumentNullValueException>(() => news.SetStatus(null!, NewsStatus.Published));
        Assert.Throws<AnotherAuthorEditNewsException>(() =>
            news.SetStatus(TestData.CreateAuthor(), NewsStatus.Published));
        Assert.False(news.SetStatus(author, NewsStatus.Created));
        Assert.True(news.SetStatus(author, NewsStatus.Published));
    }

    [Fact]
    public void News_reactions_require_published_news_and_update_existing_reactions()
    {
        var author = TestData.CreateAuthor();
        var news = TestData.CreateNews(author);
        var user = TestData.CreateUser();

        Assert.Throws<ArgumentNullValueException>(() => news.SetReaction(null!, NewsReaction.Like, DateTime.UtcNow));
        Assert.Throws<InvalidNewsStatusForUserActionException>(() =>
            news.SetReaction(user, NewsReaction.Like, DateTime.UtcNow));

        author.UpdateNewsStatus(news, NewsStatus.Published);
        Assert.True(news.SetReaction(user, NewsReaction.Like, DateTime.UtcNow));
        Assert.False(news.SetReaction(user, NewsReaction.Like, DateTime.UtcNow));
        Assert.True(news.SetReaction(user, NewsReaction.Sad, DateTime.UtcNow));
        Assert.Equal(NewsReaction.Sad, Assert.Single(news.Reactions).Type);
    }

    [Fact]
    public void News_comments_require_published_news()
    {
        var news = TestData.CreateNews();
        var user = TestData.CreateUser();
        var comment = new Comment(news, user, new CommentText("comment"), DateTime.UtcNow);

        Assert.Throws<ArgumentNullValueException>(() => news.SetComment(null!));
        Assert.Throws<InvalidNewsStatusForUserActionException>(() => news.SetComment(comment));

        news.SetStatus(news.Author, NewsStatus.Published);
        Assert.True(news.SetComment(comment));
        Assert.Same(comment, Assert.Single(news.Comments));
        Assert.Contains("comment", news.ToString());
    }

    [Fact]
    public void News_string_representation_handles_empty_and_nonempty_reactions()
    {
        var emptyNews = TestData.CreateNews();
        Assert.Contains("no comment", emptyNews.ToString());

        var author = TestData.CreateAuthor();
        var news = TestData.CreatePublishedNews(author);
        news.SetReaction(TestData.CreateUser(), NewsReaction.Laugh, DateTime.UtcNow);
        Assert.Contains("Laugh:1", news.ToString());
    }

    [Fact]
    public void User_can_change_name_and_act_on_published_news()
    {
        var user = TestData.CreateUser();
        var news = TestData.CreatePublishedNews();

        Assert.False(user.ChangeUsername(new Username("user")));
        Assert.True(user.ChangeUsername(new Username("new-user")));
        Assert.Throws<ArgumentNullValueException>(() => user.ChangeUsername(null!));
        Assert.Throws<ArgumentNullValueException>(() => user.ReactionNews(null!, NewsReaction.Like, DateTime.UtcNow));
        Assert.True(user.ReactionNews(news, NewsReaction.Like, DateTime.UtcNow));
        Assert.True(user.CommentNews(news, new CommentText("comment"), DateTime.UtcNow));
        Assert.Single(user.Comments);
        Assert.Equal("new-user", user.ToString());
    }

    [Fact]
    public void User_cannot_comment_on_unpublished_news()
    {
        var user = TestData.CreateUser();
        var news = TestData.CreateNews();

        Assert.Throws<ArgumentNullValueException>(() => user.CommentNews(null!, new CommentText("comment"), DateTime.UtcNow));
        Assert.Throws<ArgumentNullException>(() => user.CommentNews(news, null!, DateTime.UtcNow));
        Assert.Throws<InvalidNewsStatusForUserActionException>(() =>
            user.CommentNews(news, new CommentText("comment"), DateTime.UtcNow));
    }

    [Fact]
    public void Comment_content_changes_and_string_representation_are_value_based()
    {
        var comment = new Comment(
            TestData.CreateNews(),
            TestData.CreateUser(),
            new CommentText("initial"),
            DateTime.UtcNow);

        Assert.False(comment.SetContent(new CommentText("initial")));
        Assert.True(comment.SetContent(new CommentText("updated")));
        Assert.Equal("updated", comment.ToString());
        Assert.Throws<ArgumentNullValueException>(() => comment.SetContent(null!));
    }

    [Fact]
    public void Reaction_update_is_limited_to_its_owner()
    {
        var user = TestData.CreateUser();
        var reaction = new Reaction(TestData.CreateNews(), user, NewsReaction.Like, DateTime.UtcNow);
        var changedAt = reaction.CreationDate;

        reaction.UpdateType(NewsReaction.Sad, user);
        Assert.Equal(NewsReaction.Sad, reaction.Type);
        Assert.True(reaction.CreationDate >= changedAt);
        Assert.Throws<AnotherUserException>(() => reaction.UpdateType(NewsReaction.Laugh, TestData.CreateUser()));
    }
}
