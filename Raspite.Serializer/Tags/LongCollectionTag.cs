namespace Raspite.Serializer.Tags;

public sealed record LongCollectionTag(long[] Children, string Name = "") : CollectionTag<long>(Children, Name)
{
    public override byte Identifier => 12;
}