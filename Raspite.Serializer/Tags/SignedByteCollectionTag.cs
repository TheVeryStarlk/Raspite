namespace Raspite.Serializer.Tags;

/// <inheritdoc cref="Tag{T}"/>
public sealed class SignedByteCollectionTag : CollectionTag<sbyte>
{
    internal override byte Type => 7;
}