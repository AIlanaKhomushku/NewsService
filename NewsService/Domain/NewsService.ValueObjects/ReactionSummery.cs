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

