namespace Raspite.Tags;

/// <remarks>
/// All tag types inside a <see cref="ListTag{T}"/> should be the same.
/// </remarks>
public sealed class ListTag<T>(T[] value, string name = "") : Tag<T[]>(value, name) where T : Tag
{
    public override byte Identifier => List;
}