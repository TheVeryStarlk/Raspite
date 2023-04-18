namespace Raspite.Serializer.Tags;

/// <inheritdoc cref="Tag{T}"/>
public sealed class CompoundTag : CollectionTag<Tag>
{
    internal override byte Type => 10;

    /// <summary>
    /// Searches inside the whole compound tag for a matching predicate tag.
    /// </summary>
    /// <param name="name">The tag's name.</param>
    /// <typeparam name="T">The tag's type.</typeparam>
    /// <returns>The tag that matches the provided name.</returns>
    public T First<T>(string? name = null) where T : Tag
    {
        var tag = Children.First(tag =>
        {
            var typeMatches = typeof(T) == tag.GetType();

            return name is not null
                ? typeMatches && tag.Name == name
                : typeMatches;
        });

        return (T) tag;
    }

    public static implicit operator CompoundTag(Tag[] children)
    {
        return new CompoundTag()
        {
            Children = children
        };
    }
}