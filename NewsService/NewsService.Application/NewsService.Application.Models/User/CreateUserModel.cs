using NewsService.Application.Models.Base;

namespace NewsService.Application.Models.User;

public sealed class CreateUserModel : ICreateModel
{
    public string Username { get; init; } = string.Empty;
}