namespace Raspite.Serializer.Tags;

/// <inheritdoc />
public sealed class IntArrayTag : TagBase
{
    public required IEnumerable<int> Value { get; set; }
}