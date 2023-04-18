namespace Raspite.Serializer.Tags;

/// <inheritdoc cref="Tag{T}"/>
public sealed class ListTag : CollectionTag<Tag>
{
    internal override byte Type => 9;

    /// <summary>
    /// Searches inside the whole list tag for a matching predicate tag.
    /// </summary>
    /// <typeparam name="T">The tag's type.</typeparam>
    /// <returns>The tag that matches the provided name.</returns>
    public T First<T>() where T : Tag
    {
        var tag = Children.First(tag => typeof(T) == tag.GetType());
        return (T) tag;
    }

    public static implicit operator ListTag(Tag[] children)
    {
        return new ListTag()
        {
            Children = children
        };
    }
}