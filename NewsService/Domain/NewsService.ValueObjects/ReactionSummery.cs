using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NewsService.ValueObjects;

namespace NewsService.ValueObjects;

public sealed record ReactionSummery
{
    public int Like { get; init; }
    public int Laugh { get; init; }
    public int Sad { get; init; }
    public static readonly ReactionSummery Emty = new();
    public static ReactionSummery FromString(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Emty;

        var result = new ReactionSummery();

        foreach (var part in value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            var keyValue = part.Split('-', 2, StringSplitOptions.TrimEntries);
            if (keyValue.Length != 2 || !int.TryParse(keyValue[1], out var count))
                continue;

            result = keyValue[0] switch
            {
                nameof(Like) => result with { Like = count },
                nameof(Laugh) => result with { Laugh = count },
                nameof(Sad) => result with { Sad = count },
                _ => result
            };
        }

        return result;
    }
    public ReactionSummery Add(NewsReaction reaction) => reaction switch
    {
        NewsReaction.Like => this with { Like = Like + 1 },
        NewsReaction.Laugh => this with { Laugh = Laugh + 1 },
        NewsReaction.Sad => this with { Sad = Sad + 1 },
        _ => this
    };
    public override string ToString()
    {
        return $"Like-{Like},Laugh-{Laugh},Sad-{Sad}";
    }
}

