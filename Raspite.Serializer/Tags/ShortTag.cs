namespace Raspite.Serializer.Tags;

public sealed record ShortTag(short Value, string Name = "") : Tag<short>(Value, Name)
{
    public override byte Identifier => 2;
}