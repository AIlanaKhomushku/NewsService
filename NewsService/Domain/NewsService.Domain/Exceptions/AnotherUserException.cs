namespace NewsService.Domain.NewsService.Domain.Exceptions;

public class AnotherUserException(User user, Reaction reaction )
    : InvalidOperationException($"The user {user.Username} can't edit reaction by other user).")
{
    public Reaction reaction => reaction;
    public User user => user;
}