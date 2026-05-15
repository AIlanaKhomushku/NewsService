using NewsService.Domain.NewsService.ValueObjects.Base;
using NewsService.Domain.NewsService.ValueObjects.Validators;

namespace NewsService.Domain.NewsService.ValueObjects;

/// <summary>
/// содержимое комментария
/// </summary>
/// <param name="CommentText"></param>
public class CommentText(string CommentText) : ValueObject<string>(new CommentTextValidator(), CommentText);
