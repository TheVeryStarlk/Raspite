namespace Raspite.Serializer.Tags;

public sealed class CompoundTag : CollectionTag<Tag>
{
    internal override byte Type => 10;
}