using System.Collections.Immutable;

namespace Raspite.Tags;

public sealed class ListTag(ImmutableArray<Tag> value, string name = "") : Tag<ImmutableArray<Tag>>(value, name)
{
    public override byte Identifier => List;

    public Tag this[int index] => Value[index];

    public int Length => Value.Length;
}