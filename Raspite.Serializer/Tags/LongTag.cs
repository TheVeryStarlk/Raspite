namespace Raspite.Serializer.Tags;

public sealed record LongTag(long Value, string Name = "") : Tag<long>
{
    public override byte Identifier => 4;
}