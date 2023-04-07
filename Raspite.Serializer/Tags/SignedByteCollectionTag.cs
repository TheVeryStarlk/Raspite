namespace Raspite.Serializer.Tags;

public sealed class SignedByteCollectionTag : CollectionTag<sbyte>
{
    internal override byte Type => 7;
}