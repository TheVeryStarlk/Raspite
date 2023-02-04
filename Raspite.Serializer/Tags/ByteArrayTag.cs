namespace Raspite.Serializer.Tags;

/// <inheritdoc />
public sealed class ByteArrayTag : TagBase
{
    public required IEnumerable<byte> Value
    {
        get => (IEnumerable<byte>) InternalValue!;
        set => InternalValue = value;
    }
}