using Moq;
using NewsService.Application.Models.Author;
using NewsService.Application.Models.Comment;
using NewsService.Application.Models.News;
using NewsService.Application.Models.Reaction;
using NewsService.Application.Models.User;
using NewsService.Application.Services;
using NewsService.Domain.NewsService.Domain;
using NewsService.Domain.NewsService.Domain.Enums;
using NewsService.Domain.Repositories.Abstractions;
using NewsService.ValueObjects;

namespace NewsService.Tests;

public sealed class AuthorApplicationServiceTests
{
    private readonly Mock<IAuthorRepository> _repository = new();
    private readonly AuthorApplicationService _service;

    public AuthorApplicationServiceTests()
        => _service = new AuthorApplicationService(_repository.Object, TestData.CreateMapper());

    [Fact]
    public async Task Get_models_maps_all_authors()
    {
        var author = TestData.CreateAuthor();
        _repository.Setup(repository => repository.GetAllAsync(It.IsAny<CancellationToken>(), false))
            .ReturnsAsync((IEnumerable<Author>)[author]);

        var models = await _service.GetModelsAsync();

        var model = Assert.Single(models);
        Assert.Equal(author.Id, model.Id);
        Assert.Equal(author.Authorname.Value, model.Authorname);
    }

    [Fact]
    public async Task Get_by_id_returns_model_or_null()
    {
        var author = TestData.CreateAuthor();
        _repository.Setup(repository => repository.GetByIdAsync(author.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(author);
        _repository.Setup(repository => repository.GetByIdAsync(It.Is<Guid>(id => id != author.Id), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Author?)null);

        Assert.Equal(author.Id, (await _service.GetModelByIdAsync(author.Id))!.Id);
        Assert.Null(await _service.GetModelByIdAsync(Guid.NewGuid()));
    }

    [Fact]
    public async Task Get_by_name_returns_model_or_null()
    {
        var author = TestData.CreateAuthor();
        _repository.Setup(repository => repository.GetAuthorByAuthornameAsync("author", It.IsAny<CancellationToken>()))
            .ReturnsAsync(author);
        _repository.Setup(repository => repository.GetAuthorByAuthornameAsync("missing", It.IsAny<CancellationToken>()))
            .ReturnsAsync((Author?)null);

        Assert.Equal(author.Id, (await _service.GetAuthorByUsernameAsync("author"))!.Id);
        Assert.Null(await _service.GetAuthorByUsernameAsync("missing"));
    }

    [Fact]
    public async Task Create_rejects_duplicate_and_persists_new_author()
    {
        var existing = TestData.CreateAuthor();
        _repository.Setup(repository => repository.GetAuthorByAuthornameAsync("author", It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);
        Assert.Null(await _service.CreateModelAsync(new CreateAuthorModel { Authorname = "author" }));

        _repository.Setup(repository => repository.GetAuthorByAuthornameAsync("new-author", It.IsAny<CancellationToken>()))
            .ReturnsAsync((Author?)null);
        _repository.Setup(repository => repository.AddAsync(It.IsAny<Author>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Author author, CancellationToken _) => author);

        var created = await _service.CreateModelAsync(new CreateAuthorModel { Authorname = "new-author" });

        Assert.NotNull(created);
        Assert.Equal("new-author", created.Authorname);
        _repository.Verify(repository => repository.AddAsync(It.IsAny<Author>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Update_returns_false_for_missing_duplicate_or_unchanged_names()
    {
        var author = TestData.CreateAuthor();
        var duplicate = TestData.CreateAuthor();
        _repository.Setup(repository => repository.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Guid id, CancellationToken _) => id == author.Id ? author : null);
        _repository.Setup(repository => repository.GetAuthorByAuthornameAsync("duplicate", It.IsAny<CancellationToken>()))
            .ReturnsAsync(duplicate);
        _repository.Setup(repository => repository.GetAuthorByAuthornameAsync("author", It.IsAny<CancellationToken>()))
            .ReturnsAsync(author);

        Assert.False(await _service.UpdateModelAsync(new AuthorModel { Id = Guid.NewGuid(), Authorname = "missing" }));
        Assert.False(await _service.UpdateModelAsync(new AuthorModel { Id = author.Id, Authorname = "duplicate" }));
        Assert.False(await _service.UpdateModelAsync(new AuthorModel { Id = author.Id, Authorname = "author" }));
        _repository.Verify(repository => repository.UpdateAsync(It.IsAny<Author>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Update_persists_changed_author_name()
    {
        var author = TestData.CreateAuthor();
        _repository.Setup(repository => repository.GetByIdAsync(author.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(author);
        _repository.Setup(repository => repository.GetAuthorByAuthornameAsync("renamed", It.IsAny<CancellationToken>()))
            .ReturnsAsync((Author?)null);
        _repository.Setup(repository => repository.UpdateAsync(author, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        Assert.True(await _service.UpdateModelAsync(new AuthorModel { Id = author.Id, Authorname = "renamed" }));
        Assert.Equal("renamed", author.Authorname.Value);
    }

    [Fact]
    public async Task Delete_delegates_to_repository()
    {
        var id = Guid.NewGuid();
        _repository.Setup(repository => repository.DeleteAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        Assert.True(await _service.DeleteModelAsync(id));
    }
}

public sealed class UserApplicationServiceTests
{
    private readonly Mock<IUserRepository> _repository = new();
    private readonly UserApplicationService _service;

    public UserApplicationServiceTests()
        => _service = new UserApplicationService(_repository.Object, TestData.CreateMapper());

    [Fact]
    public async Task Get_models_maps_users_without_tracking()
    {
        var user = TestData.CreateUser();
        _repository.Setup(repository => repository.GetAllAsync(It.IsAny<CancellationToken>(), true))
            .ReturnsAsync((IEnumerable<User>)[user]);

        var model = Assert.Single(await _service.GetModelsAsync());

        Assert.Equal(user.Id, model.Id);
        Assert.Equal("user", model.Username);
    }

    [Fact]
    public async Task Get_by_id_and_username_return_models_or_null()
    {
        var user = TestData.CreateUser();
        _repository.Setup(repository => repository.GetByIdAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _repository.Setup(repository => repository.GetByIdAsync(It.Is<Guid>(id => id != user.Id), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);
        _repository.Setup(repository => repository.GetUserByUsernameAsync("user", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _repository.Setup(repository => repository.GetUserByUsernameAsync("missing", It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        Assert.Equal(user.Id, (await _service.GetModelByIdAsync(user.Id))!.Id);
        Assert.Null(await _service.GetModelByIdAsync(Guid.NewGuid()));
        Assert.Equal("user", (await _service.GetUserByUsernameAsync("user"))!.Username);
        Assert.Null(await _service.GetUserByUsernameAsync("missing"));
    }

    [Fact]
    public async Task Create_rejects_duplicate_and_persists_new_user()
    {
        _repository.Setup(repository => repository.GetUserByUsernameAsync("existing", It.IsAny<CancellationToken>()))
            .ReturnsAsync(TestData.CreateUser());
        Assert.Null(await _service.CreateModelAsync(new CreateUserModel { Username = "existing" }));

        _repository.Setup(repository => repository.GetUserByUsernameAsync("new-user", It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);
        _repository.Setup(repository => repository.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User user, CancellationToken _) => user);

        var created = await _service.CreateModelAsync(new CreateUserModel { Username = "new-user" });

        Assert.NotNull(created);
        Assert.Equal("new-user", created.Username);
        _repository.Verify(repository => repository.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Update_rejects_missing_duplicate_and_unchanged_names()
    {
        var user = TestData.CreateUser();
        _repository.Setup(repository => repository.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Guid id, CancellationToken _) => id == user.Id ? user : null);
        _repository.Setup(repository => repository.GetUserByUsernameAsync("duplicate", It.IsAny<CancellationToken>()))
            .ReturnsAsync(TestData.CreateUser());
        _repository.Setup(repository => repository.GetUserByUsernameAsync("user", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        Assert.False(await _service.UpdateModelAsync(new UserModel { Id = Guid.NewGuid(), Username = "missing" }));
        Assert.False(await _service.UpdateModelAsync(new UserModel { Id = user.Id, Username = "duplicate" }));
        Assert.False(await _service.UpdateModelAsync(new UserModel { Id = user.Id, Username = "user" }));
        _repository.Verify(repository => repository.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Update_and_delete_persist_changes()
    {
        var user = TestData.CreateUser();
        _repository.Setup(repository => repository.GetByIdAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _repository.Setup(repository => repository.GetUserByUsernameAsync("renamed", It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);
        _repository.Setup(repository => repository.UpdateAsync(user, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _repository.Setup(repository => repository.DeleteAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        Assert.True(await _service.UpdateModelAsync(new UserModel { Id = user.Id, Username = "renamed" }));
        Assert.Equal("renamed", user.Username.Value);
        Assert.True(await _service.DeleteModelAsync(user.Id));
    }
}

public sealed class NewsApplicationServiceTests
{
    private readonly Mock<INewsRepository> _newsRepository = new();
    private readonly Mock<IAuthorRepository> _authorRepository = new();
    private readonly NewsApplicationService _service;

    public NewsApplicationServiceTests()
        => _service = new NewsApplicationService(_newsRepository.Object, _authorRepository.Object, TestData.CreateMapper());

    [Fact]
    public async Task Get_models_and_get_by_id_map_news()
    {
        var author = TestData.CreateAuthor();
        var news = TestData.CreateNews(author);
        _newsRepository.Setup(repository => repository.GetAllAsync(It.IsAny<CancellationToken>(), true))
            .ReturnsAsync((IEnumerable<News>)[news]);
        _newsRepository.Setup(repository => repository.GetByIdAsync(news.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(news);
        _newsRepository.Setup(repository => repository.GetByIdAsync(It.Is<Guid>(id => id != news.Id), It.IsAny<CancellationToken>()))
            .ReturnsAsync((News?)null);

        var model = Assert.Single(await _service.GetModelsAsync());

        Assert.Equal(author.Id, model.AuthorId);
        Assert.Equal(news.Id, (await _service.GetModelByIdAsync(news.Id))!.Id);
        Assert.Null(await _service.GetModelByIdAsync(Guid.NewGuid()));
    }

    [Fact]
    public async Task Create_returns_null_for_missing_author_and_creates_draft_for_existing_author()
    {
        var author = TestData.CreateAuthor();
        _authorRepository.Setup(repository => repository.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Guid id, CancellationToken _) => id == author.Id ? author : null);
        _newsRepository.Setup(repository => repository.AddAsync(It.IsAny<News>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((News news, CancellationToken _) => news);

        Assert.Null(await _service.CreateModelAsync(new CreateNewsModel { AuthorId = Guid.NewGuid() }));

        var created = await _service.CreateModelAsync(new CreateNewsModel
        {
            AuthorId = author.Id,
            Title = "headline",
            Content = "news body"
        });

        Assert.NotNull(created);
        Assert.Equal(author.Id, created.AuthorId);
        Assert.Equal(NewsStatus.Created, created.NewsStatus);
    }

    [Fact]
    public async Task Update_returns_false_for_missing_entities_or_unchanged_news()
    {
        var author = TestData.CreateAuthor();
        var news = TestData.CreateNews(author);
        _newsRepository.Setup(repository => repository.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Guid id, CancellationToken _) => id == news.Id ? news : null);
        _authorRepository.Setup(repository => repository.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Guid id, CancellationToken _) => id == author.Id ? author : null);

        Assert.False(await _service.UpdateModelAsync(new NewsModel { Id = Guid.NewGuid(), AuthorId = author.Id }));
        Assert.False(await _service.UpdateModelAsync(new NewsModel { Id = news.Id, AuthorId = Guid.NewGuid() }));
        Assert.False(await _service.UpdateModelAsync(new NewsModel
        {
            Id = news.Id,
            AuthorId = author.Id,
            Title = news.Title.Value,
            Content = news.Content.Value
        }));
    }

    [Fact]
    public async Task Update_persists_changed_news()
    {
        var author = TestData.CreateAuthor();
        var news = TestData.CreateNews(author);
        _newsRepository.Setup(repository => repository.GetByIdAsync(news.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(news);
        _authorRepository.Setup(repository => repository.GetByIdAsync(author.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(author);
        _newsRepository.Setup(repository => repository.UpdateAsync(news, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        Assert.True(await _service.UpdateModelAsync(new NewsModel
        {
            Id = news.Id,
            AuthorId = author.Id,
            Title = "updated",
            Content = "updated body"
        }));
        Assert.Equal("updated", news.Title.Value);
    }

    [Fact]
    public async Task Update_status_handles_missing_and_changed_status()
    {
        var author = TestData.CreateAuthor();
        var news = TestData.CreateNews(author);
        _newsRepository.Setup(repository => repository.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Guid id, CancellationToken _) => id == news.Id ? news : null);
        _authorRepository.Setup(repository => repository.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Guid id, CancellationToken _) => id == author.Id ? author : null);
        _newsRepository.Setup(repository => repository.UpdateAsync(news, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        Assert.False(await _service.UpdateStatusAsync(Guid.NewGuid(), author.Id, NewsStatus.Published));
        Assert.False(await _service.UpdateStatusAsync(news.Id, Guid.NewGuid(), NewsStatus.Published));
        Assert.False(await _service.UpdateStatusAsync(news.Id, author.Id, NewsStatus.Created));
        Assert.True(await _service.UpdateStatusAsync(news.Id, author.Id, NewsStatus.Published));
    }

    [Fact]
    public async Task Delete_handles_missing_entities_and_deletes_owned_news()
    {
        var author = TestData.CreateAuthor();
        var news = TestData.CreateNews(author);
        _newsRepository.Setup(repository => repository.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Guid id, CancellationToken _) => id == news.Id ? news : null);
        _authorRepository.Setup(repository => repository.GetByIdAsync(author.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(author);
        _newsRepository.Setup(repository => repository.DeleteAsync(news, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        Assert.False(await _service.DeleteModelAsync(Guid.NewGuid()));
        Assert.True(await _service.DeleteModelAsync(news.Id));
        _newsRepository.Verify(repository => repository.DeleteAsync(news, It.IsAny<CancellationToken>()), Times.Once);
    }
}

public sealed class CommentApplicationServiceTests
{
    private readonly Mock<ICommentRepository> _commentRepository = new();
    private readonly Mock<INewsRepository> _newsRepository = new();
    private readonly Mock<IUserRepository> _userRepository = new();
    private readonly CommentApplicationService _service;

    public CommentApplicationServiceTests()
        => _service = new CommentApplicationService(
            _commentRepository.Object,
            _newsRepository.Object,
            _userRepository.Object,
            TestData.CreateMapper());

    [Fact]
    public async Task Get_methods_map_comments()
    {
        var news = TestData.CreatePublishedNews();
        var user = TestData.CreateUser();
        var comment = new Comment(news, user, new("comment"), DateTime.UtcNow);
        _commentRepository.Setup(repository => repository.GetAllAsync(It.IsAny<CancellationToken>(), true))
            .ReturnsAsync((IEnumerable<Comment>)[comment]);
        _commentRepository.Setup(repository => repository.GetByIdAsync(comment.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(comment);

        var model = Assert.Single(await _service.GetModelsAsync());

        Assert.Equal(news.Id, model.NewsId);
        Assert.Equal(user.Id, model.UserId);
        Assert.Equal(comment.Id, (await _service.GetModelByIdAsync(comment.Id))!.Id);
    }

    [Fact]
    public async Task Create_returns_null_for_missing_related_entities()
    {
        _newsRepository.Setup(repository => repository.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((News?)null);
        _userRepository.Setup(repository => repository.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        Assert.Null(await _service.CreateModelAsync(new CreateCommentModel
        {
            NewsId = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            Content = "comment"
        }));
    }

    [Fact]
    public async Task Create_persists_comment_on_published_news()
    {
        var news = TestData.CreatePublishedNews();
        var user = TestData.CreateUser();
        _newsRepository.Setup(repository => repository.GetByIdAsync(news.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(news);
        _userRepository.Setup(repository => repository.GetByIdAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _commentRepository.Setup(repository => repository.AddAsync(It.IsAny<Comment>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Comment comment, CancellationToken _) => comment);

        var created = await _service.CreateModelAsync(new CreateCommentModel
        {
            NewsId = news.Id,
            UserId = user.Id,
            Content = "comment"
        });

        Assert.NotNull(created);
        Assert.Equal(news.Id, created.NewsId);
        Assert.Equal(user.Id, created.UserId);
        _commentRepository.Verify(repository => repository.AddAsync(It.IsAny<Comment>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Update_and_delete_handle_missing_and_changed_comments()
    {
        var news = TestData.CreatePublishedNews();
        var comment = new Comment(news, TestData.CreateUser(), new("old"), DateTime.UtcNow);
        _commentRepository.Setup(repository => repository.GetByIdAsync(comment.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(comment);
        _commentRepository.Setup(repository => repository.GetByIdAsync(It.Is<Guid>(id => id != comment.Id), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Comment?)null);
        _commentRepository.Setup(repository => repository.UpdateAsync(comment, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _commentRepository.Setup(repository => repository.DeleteAsync(comment.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        Assert.False(await _service.UpdateModelAsync(new CommentModel { Id = Guid.NewGuid(), Content = "new" }));
        Assert.False(await _service.UpdateModelAsync(new CommentModel { Id = comment.Id, Content = "old" }));
        Assert.True(await _service.UpdateModelAsync(new CommentModel { Id = comment.Id, Content = "new" }));
        Assert.True(await _service.DeleteModelAsync(comment.Id));
    }
}

public sealed class ReactionApplicationServiceTests
{
    private readonly Mock<IReactionRepository> _reactionRepository = new();
    private readonly Mock<INewsRepository> _newsRepository = new();
    private readonly Mock<IUserRepository> _userRepository = new();
    private readonly ReactionApplicationService _service;

    public ReactionApplicationServiceTests()
        => _service = new ReactionApplicationService(
            _reactionRepository.Object,
            _newsRepository.Object,
            _userRepository.Object,
            TestData.CreateMapper());

    [Fact]
    public async Task Get_methods_map_reaction_related_ids()
    {
        var news = TestData.CreatePublishedNews();
        var user = TestData.CreateUser();
        news.SetReaction(user, NewsReaction.Like, DateTime.UtcNow);
        var reaction = Assert.Single(news.Reactions);
        _reactionRepository.Setup(repository => repository.GetAllAsync(It.IsAny<CancellationToken>(), true))
            .ReturnsAsync((IEnumerable<Reaction>)[reaction]);
        _reactionRepository.Setup(repository => repository.GetByIdAsync(reaction.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(reaction);

        var model = Assert.Single(await _service.GetModelsAsync());

        Assert.Equal(news.Id, model.NewsId);
        Assert.Equal(user.Id, model.UserId);
        Assert.Equal(reaction.Id, (await _service.GetModelByIdAsync(reaction.Id))!.Id);
    }

    [Fact]
    public async Task Create_returns_null_when_news_or_user_is_missing()
    {
        _newsRepository.Setup(repository => repository.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((News?)null);
        _userRepository.Setup(repository => repository.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        Assert.Null(await _service.CreateModelAsync(new CreateReactionModel
        {
            NewsId = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            Type = NewsReaction.Like
        }));
    }

    [Fact]
    public async Task Create_adds_new_reaction_and_rejects_duplicate_value()
    {
        var news = TestData.CreatePublishedNews();
        var user = TestData.CreateUser();
        _newsRepository.Setup(repository => repository.GetByIdAsync(news.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(news);
        _userRepository.Setup(repository => repository.GetByIdAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _reactionRepository.Setup(repository => repository.AddAsync(It.IsAny<Reaction>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Reaction reaction, CancellationToken _) => reaction);

        var created = await _service.CreateModelAsync(new CreateReactionModel
        {
            NewsId = news.Id,
            UserId = user.Id,
            Type = NewsReaction.Like
        });
        var duplicate = await _service.CreateModelAsync(new CreateReactionModel
        {
            NewsId = news.Id,
            UserId = user.Id,
            Type = NewsReaction.Like
        });

        Assert.NotNull(created);
        Assert.Equal(news.Id, created.NewsId);
        Assert.Equal(user.Id, created.UserId);
        Assert.Null(duplicate);
        _reactionRepository.Verify(repository => repository.AddAsync(It.IsAny<Reaction>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Create_updates_existing_reaction_type()
    {
        var news = TestData.CreatePublishedNews();
        var user = TestData.CreateUser();
        news.SetReaction(user, NewsReaction.Like, DateTime.UtcNow);
        _newsRepository.Setup(repository => repository.GetByIdAsync(news.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(news);
        _userRepository.Setup(repository => repository.GetByIdAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _reactionRepository.Setup(repository => repository.UpdateAsync(It.IsAny<Reaction>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var updated = await _service.CreateModelAsync(new CreateReactionModel
        {
            NewsId = news.Id,
            UserId = user.Id,
            Type = NewsReaction.Sad
        });

        Assert.NotNull(updated);
        Assert.Equal(NewsReaction.Sad, updated.Type);
        _reactionRepository.Verify(repository => repository.UpdateAsync(It.IsAny<Reaction>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Update_and_delete_handle_missing_and_existing_reactions()
    {
        var news = TestData.CreatePublishedNews();
        var user = TestData.CreateUser();
        news.SetReaction(user, NewsReaction.Like, DateTime.UtcNow);
        var reaction = Assert.Single(news.Reactions);
        _reactionRepository.Setup(repository => repository.GetByIdAsync(reaction.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(reaction);
        _userRepository.Setup(repository => repository.GetByIdAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _reactionRepository.Setup(repository => repository.GetByIdAsync(It.Is<Guid>(id => id != reaction.Id), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Reaction?)null);
        _userRepository.Setup(repository => repository.GetByIdAsync(It.Is<Guid>(id => id != user.Id), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);
        _reactionRepository.Setup(repository => repository.UpdateAsync(reaction, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _reactionRepository.Setup(repository => repository.DeleteAsync(reaction.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        Assert.False(await _service.UpdateModelAsync(new ReactionModel { Id = Guid.NewGuid(), Type = NewsReaction.Sad }));
        Assert.True(await _service.UpdateModelAsync(new ReactionModel { Id = reaction.Id, Type = NewsReaction.Sad }));
        Assert.True(await _service.DeleteModelAsync(reaction.Id));
    }
}
