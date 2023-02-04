namespace Raspite.Serializer.Tags;

/// <inheritdoc />
public sealed class IntArrayTag : TagBase
{
    public required IEnumerable<int> Value
    {
        get => (IEnumerable<int>) InternalValue!;
        set => InternalValue = value;
    }
}