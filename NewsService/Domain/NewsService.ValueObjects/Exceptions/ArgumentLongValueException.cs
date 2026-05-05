using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsService.Domain.NewsService.ValueObjects.Exceptions;

public class ArgumentLongValueException(string paramName, string value, int maxLength)
: FormatException($"The \"{paramName}\" length {value} greater than maximum allowed length {maxLength}")
{
    public string Value => value;
    public int MaxLength => maxLength;
}
