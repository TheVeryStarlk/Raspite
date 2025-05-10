namespace Raspite.Tags;

public sealed class LongTag(long value, string name = "") : Tag<long>(value, name)
{
    public override byte Identifier => Long;
}