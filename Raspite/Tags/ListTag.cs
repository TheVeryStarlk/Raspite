using System.Collections.Immutable;

using Raspite.Tags.Building;

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

    public Tag this[int index] => Value[index];

    public int Length { get; } = value.Length;

    public ImmutableArray<Tag> Elements { get; } = value.CastArray<Tag>();

    /// <summary>
    /// Converts this tag to a builder containing all the values this tag contained.
    /// It is the inverse operation of <code>ListTagBuilder{TTag}.Build()</code>
    /// </summary>
    /// <param name="name">
    /// The name of the tag about to be built.
    /// Can be used for renaming it.
    /// <code>null</code> to leave the name as it was.
    /// </param>
    /// <returns></returns>
    public ListTagBuilder<TTag> ToBuilder(string? name = null)
    {
        return new ListTagBuilder<TTag>([.. Value], name ?? Name);
    }
}