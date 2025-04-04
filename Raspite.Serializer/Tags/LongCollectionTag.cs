namespace Raspite.Serializer.Tags;

public sealed record LongCollectionTag(long[] Children, string Name = "") : CollectionTag<long>
{
    public override byte Identifier => 12;
}