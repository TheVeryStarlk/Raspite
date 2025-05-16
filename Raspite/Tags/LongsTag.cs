namespace Raspite.Tags;

public sealed class LongsTag(long[] value, string name = "") : Tag<long[]>(value, name)
{
    public override byte Identifier => Longs;
}