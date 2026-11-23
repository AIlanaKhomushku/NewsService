using Microsoft.EntityFrameworkCore;
using NewsService.Domain.NewsService.Domain;
using NewsService.Domain.NewsService.Domain.Enums;
using NewsService.Domain.Repositories.Abstractions;
using NewsService.Infrastructure.EntityFramework;
using NewsService.Infrastructure.EntityFramework.RepositoriesEF;
using NewsService.ValueObjects;

namespace NewsService.Tests;

public sealed class EntityFrameworkRepositoryTests
{
    [Fact]
    public async Task News_repository_loads_author_and_user_actions_for_lists_and_details()
    {
        var graph = await SeedGraphAsync();
        await using var context = CreateContext(graph.DatabaseName);
        var repository = new EfNewsRepository(context);

        var listItem = Assert.Single(await repository.GetAllAsync(CancellationToken.None, asNoTracking: true));
        var detail = await repository.GetByIdAsync(graph.News.Id, CancellationToken.None);

        Assert.Equal(graph.Author.Id, listItem.Author.Id);
        Assert.Equal(graph.Author.Id, detail!.Author.Id);
        Assert.Equal(graph.Comment.Id, Assert.Single(detail.Comments).Id);
        Assert.Equal(graph.Reaction.Id, Assert.Single(detail.Reactions).Id);
    }

    [Fact]
    public async Task Comment_and_reaction_repositories_load_news_and_user_for_lists()
    {
        var graph = await SeedGraphAsync();

        await using (var context = CreateContext(graph.DatabaseName))
        {
            var repository = new EfCommentRepository(context);
            var listItem = Assert.Single(await repository.GetAllAsync(CancellationToken.None, asNoTracking: true));
            var detail = await repository.GetByIdAsync(graph.Comment.Id, CancellationToken.None);

            Assert.Equal(graph.News.Id, listItem.News.Id);
            Assert.Equal(graph.User.Id, listItem.User.Id);
            Assert.Equal(graph.News.Id, detail!.News.Id);
            Assert.Equal(graph.User.Id, detail.User.Id);
        }

        await using (var context = CreateContext(graph.DatabaseName))
        {
            var repository = new EfReactionRepository(context);
            var listItem = Assert.Single(await repository.GetAllAsync(CancellationToken.None, asNoTracking: true));
            var detail = await repository.GetByIdAsync(graph.Reaction.Id, CancellationToken.None);

            Assert.Equal(graph.News.Id, listItem.News.Id);
            Assert.Equal(graph.User.Id, listItem.User.Id);
            Assert.Equal(graph.News.Id, detail!.News.Id);
            Assert.Equal(graph.User.Id, detail.User.Id);
        }
    }

    [Fact]
    public async Task Author_and_user_repositories_load_configured_navigations()
    {
        var graph = await SeedGraphAsync();
        await using var context = CreateContext(graph.DatabaseName);
        var authorRepository = new EfAuthorRepository(context);
        var userRepository = new EfUserRepository(context);

        var author = await authorRepository.GetAuthorByAuthornameAsync(graph.Author.Authorname.Value, CancellationToken.None);
        var authorById = await authorRepository.GetByIdAsync(graph.Author.Id, CancellationToken.None);
        var user = await userRepository.GetUserByUsernameAsync(graph.User.Username.Value, CancellationToken.None);
        var userById = await userRepository.GetByIdAsync(graph.User.Id, CancellationToken.None);

        Assert.Equal(graph.News.Id, Assert.Single(author!.PublishedNews).Id);
        Assert.Equal(graph.News.Id, Assert.Single(authorById!.PublishedNews).Id);
        Assert.Equal(graph.Comment.Id, Assert.Single(user!.Comments).Id);
        Assert.Equal(graph.Comment.Id, Assert.Single(userById!.Comments).Id);
    }

    [Fact]
    public async Task Generic_repository_supports_reads_add_update_and_delete()
    {
        var databaseName = Guid.NewGuid().ToString();
        await using var context = CreateContext(databaseName);
        var repository = new EfRepository<Author, Guid>(context);
        var author = TestData.CreateAuthor();

        Assert.Empty(await repository.GetAllAsync(CancellationToken.None));
        Assert.Null(await repository.GetByIdAsync(author.Id, CancellationToken.None));
        Assert.Same(author, await repository.AddAsync(author, CancellationToken.None));
        Assert.Single(await repository.GetAllAsync(CancellationToken.None, asNoTracking: true));
        Assert.NotNull(await repository.GetByIdAsync(author.Id, CancellationToken.None));

        Assert.True(await repository.UpdateAsync(author, CancellationToken.None));
        Assert.True(await repository.DeleteAsync(author, CancellationToken.None));
        Assert.False(await repository.DeleteAsync(author.Id, CancellationToken.None));

        var another = TestData.CreateAuthor();
        await repository.AddAsync(another, CancellationToken.None);
        Assert.True(await repository.DeleteAsync(another.Id, CancellationToken.None));
    }

    [Fact]
    public async Task Context_applies_all_entity_configurations()
    {
        await using var context = CreateContext(Guid.NewGuid().ToString());

        var tableNames = context.Model.GetEntityTypes()
            .Select(entity => entity.GetTableName())
            .Where(name => name is not null)
            .ToHashSet();

        Assert.Contains("Authors", tableNames);
        Assert.Contains("Users", tableNames);
        Assert.Contains("News", tableNames);
        Assert.Contains("Comments", tableNames);
        Assert.Contains("Reactions", tableNames);
    }

    private static ApplicationDbContext CreateContext(string databaseName)
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName)
            .Options;
        return new ApplicationDbContext(options);
    }

    private static async Task<SeededGraph> SeedGraphAsync()
    {
        var databaseName = Guid.NewGuid().ToString();
        var author = TestData.CreateAuthor();
        var user = TestData.CreateUser();
        var news = TestData.CreatePublishedNews(author);
        var comment = new Comment(news, user, new("comment"), DateTime.UtcNow);
        news.SetComment(comment);
        news.SetReaction(user, NewsReaction.Like, DateTime.UtcNow);
        var reaction = Assert.Single(news.Reactions);

        await using var context = CreateContext(databaseName);
        context.Authors.Add(author);
        context.Users.Add(user);
        context.Newss.Add(news);
        context.Comments.Add(comment);
        context.Reactions.Add(reaction);
        await context.SaveChangesAsync();

        return new SeededGraph(databaseName, author, user, news, comment, reaction);
    }

    private sealed record SeededGraph(
        string DatabaseName,
        Author Author,
        User User,
        News News,
        Comment Comment,
        Reaction Reaction);
}
