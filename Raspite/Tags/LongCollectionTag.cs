namespace Raspite.Tags;

public sealed class LongCollectionTag(long[] value, string name = "") : Tag<long[]>(value, name)
{
    public override byte Identifier => 12;
}