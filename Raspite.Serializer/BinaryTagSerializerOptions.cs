namespace Raspite.Serializer;

public sealed class BinaryTagSerializerOptions
{
    public int MaximumDepth { get; set; } = byte.MaxValue;
}