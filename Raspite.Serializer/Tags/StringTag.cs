namespace Raspite.Serializer.Tags;

public sealed record StringTag(string Value, string Name = "") : Tag<string>
{
    public override byte Identifier => 8;
}