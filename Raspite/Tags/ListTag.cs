namespace Raspite.Tags;

public sealed class ListTag(Tag[] value, string name = "") : Tag<Tag[]>(value, name)
{
    public override byte Identifier => 9;
}