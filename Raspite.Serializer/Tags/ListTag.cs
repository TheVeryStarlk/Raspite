namespace Raspite.Serializer.Tags;

/// <inheritdoc />
public sealed class ListTag : CollectionTagBase
{
    public required IEnumerable<TagBase> Value
    {
        get => (IEnumerable<TagBase>) InternalValue!;
        set => InternalValue = value;
    }
}