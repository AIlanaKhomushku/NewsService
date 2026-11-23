using Microsoft.AspNetCore.Mvc;
using NewsService.Application.Models.Comment;
using NewsService.Application.Services.Abstractions;

namespace NewsService.WebHost.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class CommentsController(ICommentApplicationService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CommentModel>>> Get(CancellationToken cancellationToken)
        => Ok(await service.GetModelsAsync(cancellationToken));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<CommentModel>> Get(Guid id, CancellationToken cancellationToken)
    {
        var model = await service.GetModelByIdAsync(id, cancellationToken);
        return model is null ? NotFound() : Ok(model);
    }

    [HttpPost]
    [ProducesResponseType(typeof(CommentModel), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<CommentModel>> Create(CreateCommentModel model, CancellationToken cancellationToken)
    {
        var created = await service.CreateModelAsync(model, cancellationToken);
        return created is null ? BadRequest("News or user was not found.") : CreatedAtAction(nameof(Get), new { id = created.Id }, created);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, CommentModel model, CancellationToken cancellationToken)
    {
        if (id != model.Id)
            return BadRequest();

        return await service.UpdateModelAsync(model, cancellationToken) ? NoContent() : NotFound();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        => await service.DeleteModelAsync(id, cancellationToken) ? NoContent() : NotFound();
}
