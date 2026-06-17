using System.Collections.Immutable;

namespace Raspite.Tags;

public sealed class ListTag<TTag>(ImmutableArray<TTag> value, string name = "") : Tag<ImmutableArray<TTag>>(value, name) where TTag : Tag
{
    public override byte Identifier => List;

    public TTag this[int index] => Value[index];

    public int Length => Value.Length;
}