namespace Raspite.Serializer.Tags;

/// <inheritdoc cref="Tag{T}"/>
public sealed class ListTag<T> : CollectionTag<T> where T : Tag
{
    internal override byte Type => 9;

    /// <summary>
    /// Searches inside the whole list tag for a matching predicate tag.
    /// </summary>
    /// <param name="name">The tag's name.</param>
    /// <returns>The tag that matches the provided name.</returns>
    public T First(string name)
    {
        var tag = Children.First(tag => tag.Name == name);
        return tag;
    }
}