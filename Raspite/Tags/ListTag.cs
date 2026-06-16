using System.Collections.Immutable;

namespace Raspite.Tags;

public sealed class ListTag(ImmutableArray<Tag> value, string name = "") : Tag<ImmutableArray<Tag>>(value, name)
{
    public override byte Identifier => List;

    public Tag this[int index] => Value[index];

    public int Length => Value.Length;

    public TTag Get<TTag>(int index) where TTag : Tag
    {
        return (TTag) Value[index];
    }

    public T GetValue<T>(int index)
    {
        return ((Tag<T>) Value[index]).Value;
    }
}