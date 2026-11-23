using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using NewsService.Domain.NewsService.Domain.Exceptions;

namespace NewsService.WebHost.Helpers;

public sealed class InvalidNewsStatusExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is not InvalidNewsStatusForUserActionException statusException)
            return false;

        httpContext.Response.StatusCode = StatusCodes.Status409Conflict;
        await httpContext.Response.WriteAsJsonAsync(
            new ProblemDetails
            {
                Status = StatusCodes.Status409Conflict,
                Title = "News is not published",
                Detail = statusException.Message,
                Instance = httpContext.Request.Path
            },
            cancellationToken);

        return true;
    }
}
