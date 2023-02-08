namespace Raspite.Serializer.Tags;

/// <inheritdoc />
public sealed class StringTag : TagBase
{
    public required string Value { get; set; }
}