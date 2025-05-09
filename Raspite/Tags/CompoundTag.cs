namespace Raspite.Tags;

public sealed class CompoundTag(Tag[] value, string name = "") : Tag<Tag[]>(value, name)
{
    public override byte Identifier => Compound;
}