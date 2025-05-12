namespace Raspite.Tags;

/// <remarks>
/// All tag types inside a <see cref="ListTag"/> should be the same.
/// </remarks>
public sealed class ListTag(Tag[] value, string name = "") : Tag<Tag[]>(value, name)
{
    public override byte Identifier => List;
}