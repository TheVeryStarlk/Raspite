namespace Raspite.Serializer.Tags;

/// <inheritdoc cref="Tag{T}"/>
public sealed class IntegerCollectionTag : CollectionTag<int>
{
    internal override byte Type => 11;
}