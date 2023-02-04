namespace Raspite.Serializer.Tags;

/// <inheritdoc />
public sealed class ByteTag : TagBase
{
    public required byte Value
    {
        get => (byte) InternalValue!;
        set => InternalValue = value;
    }
}