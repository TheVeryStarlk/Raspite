namespace Raspite.Serializer.Tags;

public sealed record ListTag(Tag[] Children, string Name = "") : CollectionTag<Tag>
{
    public override byte Identifier => 9;
}