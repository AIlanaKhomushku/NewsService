
namespace NewsService.Domain.NewsService.Domain.Exceptions
{
    public class ArgumentNullValueException(string paramName)
    : ArgumentNullException(paramName, $"Argument \"{paramName}\" value is null");
}
