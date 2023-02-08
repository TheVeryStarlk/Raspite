namespace Raspite.Serializer.Tags;

/// <inheritdoc />
public sealed class CompoundTag : CollectionTagBase
{
    public required IEnumerable<TagBase> Value { get; set; }
}