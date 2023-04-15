namespace Raspite.Serializer.Tags;

/// <inheritdoc cref="Tag{T}"/>
public sealed class LongCollectionTag : CollectionTag<long>
{
    internal override byte Type => 12;
}