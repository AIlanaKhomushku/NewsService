using NewsService.Domain.NewsService.ValueObjects;
using NewsService.Domain.NewsService.ValueObjects.Base;
using NewsService.Domain.NewsService.ValueObjects.Exceptions;

namespace NewsService.Tests;

public sealed class ValueObjectTests
{
    [Fact]
    public void Value_objects_keep_validated_values()
    {
        Assert.Equal("headline", new Title("headline").Value);
        Assert.Equal("valid content", new Content("valid content").Value);
        Assert.Equal("comment", new CommentText("comment").Value);
        Assert.Equal("author", new Authorname("author").Value);
        Assert.Equal("username", new Username("username").Value);
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    [InlineData(null)]
    public void String_value_objects_reject_null_or_whitespace(string? value)
    {
        Assert.Throws<ArgumentNullOrWhiteSpaceException>(() => new Title(value!));
        Assert.Throws<ArgumentNullOrWhiteSpaceException>(() => new Content(value!));
        Assert.Throws<ArgumentNullOrWhiteSpaceException>(() => new CommentText(value!));
        Assert.Throws<ArgumentNullOrWhiteSpaceException>(() => new Authorname(value!));
        Assert.Throws<ArgumentNullOrWhiteSpaceException>(() => new Username(value!));
    }

    [Fact]
    public void Value_objects_reject_values_over_their_limits()
    {
        Assert.Throws<ArgumentLongValueException>(() => new Title(new string('t', 51)));
        Assert.Throws<ArgumentLongValueException>(() => new Content(new string('c', 50_001)));
        Assert.Throws<ArgumentLongValueException>(() => new CommentText(new string('c', 201)));
        Assert.Throws<ArgumentLongValueException>(() => new Authorname(new string('a', 31)));
        Assert.Throws<ArgumentLongValueException>(() => new Username(new string('u', 31)));
    }

    [Fact]
    public void Content_rejects_values_shorter_than_its_minimum()
    {
        Assert.Throws<ArgumentShortValueException>(() => new Content("ab"));
    }

    [Fact]
    public void Value_objects_compare_by_type_and_value()
    {
        var firstTitle = new Title("headline");
        var sameTitle = new Title("headline");
        var otherTitle = new Title("another");

        Assert.True(firstTitle.Equals(sameTitle));
        Assert.True(firstTitle == sameTitle);
        Assert.False(firstTitle != sameTitle);
        Assert.Equal(firstTitle.GetHashCode(), sameTitle.GetHashCode());
        Assert.False(firstTitle.Equals(otherTitle));
        Assert.False(firstTitle.Equals(new Content("headline")));
        Assert.False(firstTitle.Equals(null));
        Assert.False(firstTitle.Equals(new object()));
        var sameReference = firstTitle;
        Assert.True(firstTitle == sameReference);
        Assert.False(firstTitle == null);
        Assert.True(firstTitle != null);
        Assert.Equal("headline", firstTitle.ToString());
    }

    [Fact]
    public void Value_object_requires_a_validator()
    {
        Assert.Throws<ValidatorNullException>(() => new TestValueObject(null!, "value"));
    }

    private sealed class TestValueObject(IValidator<string> validator, string value)
        : ValueObject<string>(validator, value);
}
