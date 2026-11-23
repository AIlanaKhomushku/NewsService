using Microsoft.AspNetCore.Mvc;
using NewsService.Application.Models.Reaction;
using NewsService.Application.Services.Abstractions;

namespace NewsService.WebHost.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class ReactionsController(IReactionApplicationService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ReactionModel>>> Get(CancellationToken cancellationToken)
        => Ok(await service.GetModelsAsync(cancellationToken));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ReactionModel>> Get(Guid id, CancellationToken cancellationToken)
    {
        var model = await service.GetModelByIdAsync(id, cancellationToken);
        return model is null ? NotFound() : Ok(model);
    }

    [HttpPost]
    [ProducesResponseType(typeof(ReactionModel), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ReactionModel>> Create(CreateReactionModel model, CancellationToken cancellationToken)
    {
        var created = await service.CreateModelAsync(model, cancellationToken);
        return created is null ? BadRequest("News or user was not found, or news is not published.") : CreatedAtAction(nameof(Get), new { id = created.Id }, created);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, ReactionModel model, CancellationToken cancellationToken)
    {
        if (id != model.Id)
            return BadRequest();

        return await service.UpdateModelAsync(model, cancellationToken) ? NoContent() : NotFound();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        => await service.DeleteModelAsync(id, cancellationToken) ? NoContent() : NotFound();
}
