using System.Collections.Immutable;

namespace Raspite.Tags;

public interface IListTag
{
    string Name { get; }

    byte Identifier { get; }

    byte ElementIdentifier { get; }

    int Length { get; }

    ImmutableArray<Tag> Elements { get; }
}

public sealed class ListTag<TTag>(ImmutableArray<TTag> value, string name = "") : Tag<ImmutableArray<TTag>>(value, name), IListTag where TTag : Tag
{
    public override byte Identifier => List;

    public byte ElementIdentifier { get; } = value.Length > 0 ? value[0].Identifier : End;

    public TTag this[int index] => Value[index];

    public int Length { get; } = value.Length;

    public ImmutableArray<Tag> Elements { get; } = value.CastArray<Tag>();
}