using NewsService.Application.Models.Base;

namespace NewsService.Application.Models.User;

public sealed class UserModel : IModel<Guid>
{
    public Guid Id { get; init; }
    public string Username { get; init; } = string.Empty;
}