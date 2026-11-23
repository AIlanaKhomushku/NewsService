using Microsoft.AspNetCore.Mvc;
using NewsService.Application.Models.Author;
using NewsService.Application.Services.Abstractions;

namespace NewsService.WebHost.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class AuthorsController(IAuthorApplicationService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<AuthorModel>>> Get(CancellationToken cancellationToken)
        => Ok(await service.GetModelsAsync(cancellationToken));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<AuthorModel>> Get(Guid id, CancellationToken cancellationToken)
    {
        var model = await service.GetModelByIdAsync(id, cancellationToken);
        return model is null ? NotFound() : Ok(model);
    }

    [HttpPost]
    public async Task<ActionResult<AuthorModel>> Create(CreateAuthorModel model, CancellationToken cancellationToken)
    {
        var created = await service.CreateModelAsync(model, cancellationToken);
        return created is null ? Conflict("An author with this authorname already exists.") : CreatedAtAction(nameof(Get), new { id = created.Id }, created);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, AuthorModel model, CancellationToken cancellationToken)
    {
        if (id != model.Id)
            return BadRequest();

        return await service.UpdateModelAsync(model, cancellationToken) ? NoContent() : NotFound();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        => await service.DeleteModelAsync(id, cancellationToken) ? NoContent() : NotFound();
}
