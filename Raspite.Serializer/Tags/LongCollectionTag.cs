namespace Raspite.Serializer.Tags;

public sealed class LongCollectionTag : CollectionTag<long>
{
    internal override byte Type => 12;
}