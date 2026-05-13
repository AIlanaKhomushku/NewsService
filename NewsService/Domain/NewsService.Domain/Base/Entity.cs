

namespace NewsService.Domain.NewsService.Domain.Base;

public abstract class Entity<TId>(TId id) where TId : struct, IEquatable<TId>
{
    /// <summary>
    /// Gets the ID of the entity.
    /// </summary>
    public TId Id { get; } = id;
    /// <summary>
    /// Protected constructor for entity framework if needed.
    /// </summary>
    protected Entity() : this(default!)
    {

    }
}
