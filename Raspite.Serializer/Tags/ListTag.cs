namespace Raspite.Serializer.Tags;

public sealed record ListTag(Tag[] Children, string Name = "") : CollectionTag<Tag>(Children, Name)
{
    public override byte Identifier => 9;
}