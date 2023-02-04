namespace Raspite.Serializer.Tags;

/// <inheritdoc />
public sealed class ShortTag : TagBase
{
    public required short Value
    {
        get => (short) InternalValue!;
        set => InternalValue = value;
    }
}