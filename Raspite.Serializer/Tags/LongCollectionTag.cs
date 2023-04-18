namespace Raspite.Serializer.Tags;

/// <inheritdoc cref="Tag{T}"/>
public sealed class LongCollectionTag : CollectionTag<long>
{
    internal override byte Type => 12;

    public static implicit operator LongCollectionTag(long[] children)
    {
        return new LongCollectionTag()
        {
            Children = children
        };
    }
}