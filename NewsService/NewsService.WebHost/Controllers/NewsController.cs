using Microsoft.AspNetCore.Mvc;
using NewsService.Application.Models.News;
using NewsService.Application.Services.Abstractions;
using NewsService.Domain.NewsService.Domain.Enums;

namespace NewsService.WebHost.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class NewsController(INewsApplicationService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<NewsModel>>> Get(CancellationToken cancellationToken)
        => Ok(await service.GetModelsAsync(cancellationToken));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<NewsModel>> Get(Guid id, CancellationToken cancellationToken)
    {
        var model = await service.GetModelByIdAsync(id, cancellationToken);
        return model is null ? NotFound() : Ok(model);
    }

    [HttpPost]
    public async Task<ActionResult<NewsModel>> Create(CreateNewsModel model, CancellationToken cancellationToken)
    {
        var created = await service.CreateModelAsync(model, cancellationToken);
        return created is null ? BadRequest("Author was not found.") : CreatedAtAction(nameof(Get), new { id = created.Id }, created);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, NewsModel model, CancellationToken cancellationToken)
    {
        if (id != model.Id)
            return BadRequest();

        return await service.UpdateModelAsync(model, cancellationToken) ? NoContent() : NotFound();
    }

    [HttpPatch("{id:guid}/status")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateStatus(Guid id, Guid authorId, NewsStatus status, CancellationToken cancellationToken)
        => await service.UpdateStatusAsync(id, authorId, status, cancellationToken) ? NoContent() : NotFound();

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        => await service.DeleteModelAsync(id, cancellationToken) ? NoContent() : NotFound();
}
