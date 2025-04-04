namespace Raspite.Serializer.Tags;

public sealed record ShortTag(short Value, string Name = "") : Tag<short>
{
    public override byte Identifier => 2;
}