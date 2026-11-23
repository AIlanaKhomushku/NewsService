using Microsoft.AspNetCore.Mvc;
using Moq;
using NewsService.Application.Models.Author;
using NewsService.Application.Models.Comment;
using NewsService.Application.Models.News;
using NewsService.Application.Models.Reaction;
using NewsService.Application.Models.User;
using NewsService.Application.Services.Abstractions;
using NewsService.Domain.NewsService.Domain.Enums;
using NewsService.ValueObjects;
using NewsService.WebHost.Controllers;

namespace NewsService.Tests;

public sealed class AuthorsControllerTests
{
    [Fact]
    public async Task Author_controller_returns_expected_crud_status_codes()
    {
        var id = Guid.NewGuid();
        var model = new AuthorModel { Id = id, Authorname = "author" };
        var service = new Mock<IAuthorApplicationService>();
        service.Setup(item => item.GetModelsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync((IEnumerable<AuthorModel>)[model]);
        service.Setup(item => item.GetModelByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(model);
        service.Setup(item => item.GetModelByIdAsync(It.Is<Guid>(value => value != id), It.IsAny<CancellationToken>()))
            .ReturnsAsync((AuthorModel?)null);
        service.Setup(item => item.CreateModelAsync(It.IsAny<CreateAuthorModel>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((CreateAuthorModel input, CancellationToken _) =>
                input.Authorname == "duplicate" ? null : model);
        service.Setup(item => item.UpdateModelAsync(model, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        service.Setup(item => item.DeleteModelAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        service.Setup(item => item.DeleteModelAsync(It.Is<Guid>(value => value != id), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        var controller = new AuthorsController(service.Object);

        Assert.IsType<OkObjectResult>((await controller.Get(CancellationToken.None)).Result);
        Assert.IsType<OkObjectResult>((await controller.Get(id, CancellationToken.None)).Result);
        Assert.IsType<NotFoundResult>((await controller.Get(Guid.NewGuid(), CancellationToken.None)).Result);
        Assert.IsType<CreatedAtActionResult>((await controller.Create(new() { Authorname = "new" }, CancellationToken.None)).Result);
        Assert.IsType<ConflictObjectResult>((await controller.Create(new() { Authorname = "duplicate" }, CancellationToken.None)).Result);
        Assert.IsType<BadRequestResult>(await controller.Update(Guid.NewGuid(), model, CancellationToken.None));
        Assert.IsType<NoContentResult>(await controller.Update(id, model, CancellationToken.None));
        Assert.IsType<NoContentResult>(await controller.Delete(id, CancellationToken.None));
        Assert.IsType<NotFoundResult>(await controller.Delete(Guid.NewGuid(), CancellationToken.None));
    }
}

public sealed class UsersControllerTests
{
    [Fact]
    public async Task User_controller_returns_expected_crud_status_codes()
    {
        var id = Guid.NewGuid();
        var model = new UserModel { Id = id, Username = "user" };
        var service = new Mock<IUserApplicationService>();
        service.Setup(item => item.GetModelsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync((IEnumerable<UserModel>)[model]);
        service.Setup(item => item.GetModelByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(model);
        service.Setup(item => item.GetModelByIdAsync(It.Is<Guid>(value => value != id), It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserModel?)null);
        service.Setup(item => item.CreateModelAsync(It.IsAny<CreateUserModel>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((CreateUserModel input, CancellationToken _) =>
                input.Username == "duplicate" ? null : model);
        service.Setup(item => item.UpdateModelAsync(model, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        service.Setup(item => item.DeleteModelAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        service.Setup(item => item.DeleteModelAsync(It.Is<Guid>(value => value != id), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        var controller = new UsersController(service.Object);

        Assert.IsType<OkObjectResult>((await controller.Get(CancellationToken.None)).Result);
        Assert.IsType<OkObjectResult>((await controller.Get(id, CancellationToken.None)).Result);
        Assert.IsType<NotFoundResult>((await controller.Get(Guid.NewGuid(), CancellationToken.None)).Result);
        Assert.IsType<CreatedAtActionResult>((await controller.Create(new() { Username = "new-user" }, CancellationToken.None)).Result);
        Assert.IsType<ConflictObjectResult>((await controller.Create(new() { Username = "duplicate" }, CancellationToken.None)).Result);
        Assert.IsType<BadRequestResult>(await controller.Update(Guid.NewGuid(), model, CancellationToken.None));
        Assert.IsType<NoContentResult>(await controller.Update(id, model, CancellationToken.None));
        Assert.IsType<NoContentResult>(await controller.Delete(id, CancellationToken.None));
        Assert.IsType<NotFoundResult>(await controller.Delete(Guid.NewGuid(), CancellationToken.None));
    }
}

public sealed class NewsControllerTests
{
    [Fact]
    public async Task News_controller_returns_expected_crud_and_status_codes()
    {
        var id = Guid.NewGuid();
        var authorId = Guid.NewGuid();
        var model = new NewsModel { Id = id, AuthorId = authorId, Title = "headline", Content = "news body" };
        var service = new Mock<INewsApplicationService>();
        service.Setup(item => item.GetModelsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync((IEnumerable<NewsModel>)[model]);
        service.Setup(item => item.GetModelByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(model);
        service.Setup(item => item.GetModelByIdAsync(It.Is<Guid>(value => value != id), It.IsAny<CancellationToken>()))
            .ReturnsAsync((NewsModel?)null);
        service.Setup(item => item.CreateModelAsync(It.IsAny<CreateNewsModel>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((CreateNewsModel input, CancellationToken _) => input.AuthorId == authorId ? model : null);
        service.Setup(item => item.UpdateModelAsync(model, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        service.Setup(item => item.UpdateStatusAsync(id, authorId, NewsStatus.Published, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        service.Setup(item => item.UpdateStatusAsync(It.Is<Guid>(value => value != id), It.IsAny<Guid>(), It.IsAny<NewsStatus>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        service.Setup(item => item.DeleteModelAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        service.Setup(item => item.DeleteModelAsync(It.Is<Guid>(value => value != id), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        var controller = new NewsController(service.Object);

        Assert.IsType<OkObjectResult>((await controller.Get(CancellationToken.None)).Result);
        Assert.IsType<OkObjectResult>((await controller.Get(id, CancellationToken.None)).Result);
        Assert.IsType<NotFoundResult>((await controller.Get(Guid.NewGuid(), CancellationToken.None)).Result);
        Assert.IsType<CreatedAtActionResult>((await controller.Create(
            new() { AuthorId = authorId, Title = "headline", Content = "news body" },
            CancellationToken.None)).Result);
        Assert.IsType<BadRequestObjectResult>((await controller.Create(new(), CancellationToken.None)).Result);
        Assert.IsType<BadRequestResult>(await controller.Update(Guid.NewGuid(), model, CancellationToken.None));
        Assert.IsType<NoContentResult>(await controller.Update(id, model, CancellationToken.None));
        Assert.IsType<NoContentResult>(await controller.UpdateStatus(id, authorId, NewsStatus.Published, CancellationToken.None));
        Assert.IsType<NotFoundResult>(await controller.UpdateStatus(Guid.NewGuid(), authorId, NewsStatus.Published, CancellationToken.None));
        Assert.IsType<NoContentResult>(await controller.Delete(id, CancellationToken.None));
        Assert.IsType<NotFoundResult>(await controller.Delete(Guid.NewGuid(), CancellationToken.None));
    }
}

public sealed class CommentsControllerTests
{
    [Fact]
    public async Task Comment_controller_returns_expected_crud_status_codes()
    {
        var id = Guid.NewGuid();
        var model = new CommentModel { Id = id, Content = "comment" };
        var service = new Mock<ICommentApplicationService>();
        service.Setup(item => item.GetModelsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync((IEnumerable<CommentModel>)[model]);
        service.Setup(item => item.GetModelByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(model);
        service.Setup(item => item.GetModelByIdAsync(It.Is<Guid>(value => value != id), It.IsAny<CancellationToken>()))
            .ReturnsAsync((CommentModel?)null);
        service.Setup(item => item.CreateModelAsync(It.IsAny<CreateCommentModel>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((CreateCommentModel input, CancellationToken _) => input.Content == "comment" ? model : null);
        service.Setup(item => item.UpdateModelAsync(model, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        service.Setup(item => item.DeleteModelAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        service.Setup(item => item.DeleteModelAsync(It.Is<Guid>(value => value != id), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        var controller = new CommentsController(service.Object);

        Assert.IsType<OkObjectResult>((await controller.Get(CancellationToken.None)).Result);
        Assert.IsType<OkObjectResult>((await controller.Get(id, CancellationToken.None)).Result);
        Assert.IsType<NotFoundResult>((await controller.Get(Guid.NewGuid(), CancellationToken.None)).Result);
        Assert.IsType<CreatedAtActionResult>((await controller.Create(new() { Content = "comment" }, CancellationToken.None)).Result);
        Assert.IsType<BadRequestObjectResult>((await controller.Create(new(), CancellationToken.None)).Result);
        Assert.IsType<BadRequestResult>(await controller.Update(Guid.NewGuid(), model, CancellationToken.None));
        Assert.IsType<NoContentResult>(await controller.Update(id, model, CancellationToken.None));
        Assert.IsType<NoContentResult>(await controller.Delete(id, CancellationToken.None));
        Assert.IsType<NotFoundResult>(await controller.Delete(Guid.NewGuid(), CancellationToken.None));
    }
}

public sealed class ReactionsControllerTests
{
    [Fact]
    public async Task Reaction_controller_returns_expected_crud_status_codes()
    {
        var id = Guid.NewGuid();
        var model = new ReactionModel { Id = id, Type = NewsReaction.Like };
        var service = new Mock<IReactionApplicationService>();
        service.Setup(item => item.GetModelsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync((IEnumerable<ReactionModel>)[model]);
        service.Setup(item => item.GetModelByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(model);
        service.Setup(item => item.GetModelByIdAsync(It.Is<Guid>(value => value != id), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ReactionModel?)null);
        service.Setup(item => item.CreateModelAsync(It.IsAny<CreateReactionModel>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((CreateReactionModel input, CancellationToken _) => input.NewsId == id ? model : null);
        service.Setup(item => item.UpdateModelAsync(model, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        service.Setup(item => item.DeleteModelAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        service.Setup(item => item.DeleteModelAsync(It.Is<Guid>(value => value != id), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        var controller = new ReactionsController(service.Object);

        Assert.IsType<OkObjectResult>((await controller.Get(CancellationToken.None)).Result);
        Assert.IsType<OkObjectResult>((await controller.Get(id, CancellationToken.None)).Result);
        Assert.IsType<NotFoundResult>((await controller.Get(Guid.NewGuid(), CancellationToken.None)).Result);
        Assert.IsType<CreatedAtActionResult>((await controller.Create(new() { NewsId = id }, CancellationToken.None)).Result);
        Assert.IsType<BadRequestObjectResult>((await controller.Create(new(), CancellationToken.None)).Result);
        Assert.IsType<BadRequestResult>(await controller.Update(Guid.NewGuid(), model, CancellationToken.None));
        Assert.IsType<NoContentResult>(await controller.Update(id, model, CancellationToken.None));
        Assert.IsType<NoContentResult>(await controller.Delete(id, CancellationToken.None));
        Assert.IsType<NotFoundResult>(await controller.Delete(Guid.NewGuid(), CancellationToken.None));
    }
}
