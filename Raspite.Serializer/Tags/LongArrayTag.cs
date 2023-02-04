namespace Raspite.Serializer.Tags;

/// <inheritdoc />
public sealed class LongArrayTag : TagBase
{
    public required IEnumerable<long> Value
    {
        get => (IEnumerable<long>) InternalValue!;
        set => InternalValue = value;
    }
}