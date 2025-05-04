namespace Raspite;

public static class BinaryTagSerializer
{
}

public sealed class BinaryTagSerializerOptions
{
    public bool LittleEndian { get; init; }

    public int MaximumDepth { get; init; } = 256;

    public int MinimumLength { get; init; } = 2048;
}