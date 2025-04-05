namespace Raspite.Serializer.Tags;

public sealed record CompoundTag(Tag[] Children, string Name = "") : CollectionTag<Tag>(Children, Name)
{
    public override byte Identifier => 10;
}