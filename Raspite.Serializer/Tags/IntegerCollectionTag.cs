namespace Raspite.Serializer.Tags;

/// <inheritdoc cref="Tag{T}"/>
public sealed class IntegerCollectionTag : CollectionTag<int>
{
    internal override byte Type => 11;

    public static implicit operator IntegerCollectionTag(int[] children)
    {
        return new IntegerCollectionTag()
        {
            Children = children
        };
    }
}