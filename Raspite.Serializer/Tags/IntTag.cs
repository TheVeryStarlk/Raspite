namespace Raspite.Serializer.Tags;

/// <inheritdoc />
public sealed class IntTag : TagBase
{
    public required int Value
    {
        get => (int) InternalValue!;
        set => InternalValue = value;
    }
}