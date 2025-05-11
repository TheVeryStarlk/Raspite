namespace Raspite.Tags;

public sealed class ListTag<T>(T[] value, string name = "") : Tag<T[]>(value, name) where T : Tag
{
    public override byte Identifier => List;
}