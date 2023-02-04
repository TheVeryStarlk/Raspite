namespace Raspite.Serializer.Tags;

/// <inheritdoc />
public sealed class LongTag : TagBase
{
    public required long Value
    {
        get => (long) InternalValue!;
        set => InternalValue = value;
    }
}