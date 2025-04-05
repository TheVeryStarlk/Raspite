namespace Raspite.Serializer.Tags;

public sealed record LongTag(long Value, string Name = "") : Tag<long>(Value, Name)
{
    public override byte Identifier => 4;
}