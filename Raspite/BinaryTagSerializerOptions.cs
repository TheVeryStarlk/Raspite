namespace Raspite;

public readonly struct BinaryTagSerializerOptions()
{
    public static BinaryTagSerializerOptions Default => new();

    public int MaximumDepth { get; init; } = 512;

    public bool LittleEndian { get; init; } = false;
}