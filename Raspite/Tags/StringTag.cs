namespace Raspite.Tags;

public sealed class StringTag(string value, string name = "") : Tag<string>(value, name)
{
    public override byte Identifier => 8;
}